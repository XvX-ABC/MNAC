using Assets.Scripts.Utilities;
using Locomotion;
using System;
using Tests.Environment;
using Unity.VisualScripting;
using UnityEngine;
using JState = Tests.Locomotion.JumpLocomotion.State;
namespace Tests.Locomotion
{
    class AirLocomotion : IModule
    {

        IBaseDefinitions _definitions;
        JumpLocomotion _jumpLocomotion;
        Action<Context> _startAction;
        Action<Context> _endAction;
        StateEvent<bool, Context> _stateEvent;
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
        }

        public Action<Context> StartAction { get => _startAction; set => _startAction = value; }
        public Action<Context> EndAction { get => _endAction; set => _endAction = value; }

        public void OnFixedUpdate(Context context)
        {
            var input = context.Input;
            var ground = context.Ground;
            if (ground != null)
            {
                _stateEvent.TryExecute(false, context);
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

        }
    }
}