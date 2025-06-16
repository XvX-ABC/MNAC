using Assets.Scripts.Utilities;
using Assets.Scripts.Utilities.Timeline;
using Locomotion;
using System;
using Tests.Environment;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using JState = Tests.Locomotion.JumpLocomotion.State;
namespace Tests.Locomotion
{
    class AirLocomotion : IModule
    {
        internal class PostureEvaluator
        {
            public enum State
            {
                Unchanged = 0,
                Ascending = 1,
                Descending = 2,
            }
            const byte EVALUATION_FRAMES = 3;
            float[] _velocityCaches;
            byte _index;
            State _stateCache;
            public Action<byte, Context> PostureChangedAction;
            public Action<Context> AscendingAction;
            public Action<Context> DescendingAction;
            public PostureEvaluator()
            {
                _velocityCaches = new float[EVALUATION_FRAMES];
            }
            public void WriteVelocityCache(float velocity)
            {
                _velocityCaches[_index] = velocity;
                _index++;
                if (_index >= EVALUATION_FRAMES)
                    _index = 0;
            }
            public State AscendingEvaluate()
            {
                var sum = 0f;
                for (int i = 0; i < EVALUATION_FRAMES; i++)
                {
                    sum += _velocityCaches[i];
                }
                sum /= EVALUATION_FRAMES;
                if (sum == 0)
                    return State.Unchanged;
                else
                    return sum > 0 ? State.Ascending : State.Descending;
            }
            public void OnFixedUpdate(Context context)
            {
                var state = AscendingEvaluate();
                if (state != _stateCache)
                {
                    PostureChangedAction?.Invoke((byte)state, context);
                }
                if (state == State.Ascending)
                {
                    AscendingAction?.Invoke(context);
                }
                else if (state == State.Descending)
                {
                    DescendingAction?.Invoke(context);
                }
                _stateCache = state;
                WriteVelocityCache(context.Velocity.y);
            }
            public void Reset()
            {
                Array.Clear(_velocityCaches, 0, _velocityCaches.Length);
                _stateCache = State.Unchanged;
            }
        }

        IBaseDefinitions _definitions;
        JumpLocomotion _jumpLocomotion;
        Action<Context> _startAction;
        Action<Context> _endAction;
        StateEvent<bool, Context> _stateEvent;
        PostureEvaluator _evaluator;

        public AirLocomotion(IBaseDefinitions definition, JumpLocomotion jumpLocomotion)
        {
            _definitions = definition ?? throw new ArgumentNullException(nameof(definition));
            _jumpLocomotion = jumpLocomotion ?? throw new ArgumentNullException(nameof(jumpLocomotion));
            _stateEvent = new((os, inAir, ctx) =>
            {
                if (inAir)
                {
                    _startAction?.Invoke(ctx);
                }
                else if (!inAir)
                {
                    _endAction?.Invoke(ctx);
                }
            });
            _evaluator = new PostureEvaluator();
        }

        public Action<Context> StartAction { get => _startAction; set => _startAction = value; }
        public Action<Context> EndAction { get => _endAction; set => _endAction = value; }
        public Action<byte, Context> PostureChangedAction
        {
            get => _evaluator.PostureChangedAction;
            set => _evaluator.PostureChangedAction = value;
        }
        public Action<Context> AscendingAction
        {
            get => _evaluator.AscendingAction;
            set => _evaluator.AscendingAction = value;
        }
        public Action<Context> DescendingAction
        {
            get => _evaluator.DescendingAction;
            set => _evaluator.DescendingAction = value;
        }
        public void OnFixedUpdate(Context context)
        {
            var input = context.Input;
            var ground = context.Ground;
            if (ground != null)
            {
                _stateEvent.TryExecute(false, context);
                _evaluator.Reset();
                return;
            }

            if (input.IsAscending)
            {

                if (_jumpLocomotion.CurrentState > JState.OnGround && _jumpLocomotion.CurrentState <= JState.Ascending)
                    return;
                else if (_jumpLocomotion.CurrentState == JState.Descending)
                    _jumpLocomotion.EndJump(context);

                var velocity = context.Velocity;
                var y = velocity.y;
                if (y <= _definitions.AscendingSpeed)
                    velocity.y = _definitions.AscendingSpeed;
                context.Velocity = velocity;
            }
            _stateEvent.TryExecute(true, context);
            _evaluator.OnFixedUpdate(context);
        }
    }
}