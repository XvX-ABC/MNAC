using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using Locomotion;
using System;
using System.Collections.Generic;
using Tests.Environment;
using UnityEngine;
using UnityEngine.Windows;
namespace Tests.Locomotion
{
    class JumpLocomotion : IModule
    {
        public enum State
        {
            OnGround,
            InPreparation,
            Ascending,
            Descending
        }
        Vector3 _startVelocity;
        float _ascendingDurationTime;
        ITimeline _ascendingTimeline;
        ITimeline _preparationTimeline;
        State _state;
        Vector3[] _probes;
        List<RaycastHit> _probeResults;
        IJumpDefinitions _definitions;
        Context _context;
        Action<State, Context> _preparationStartAction;
        Action<State, Context> _preparationEndAction;
        Action<State, Context> _ascendingStartAction;
        Action<State, Context> _ascendingEndAction;
        Action<State, Context> _jumpEndAction;
        public State CurrentState => _state;
        internal float ascendingDurationTime => _ascendingDurationTime;
        internal IJumpDefinitions definitions => _definitions;

        internal Action<State, Context> PreparationStartAction { get => _preparationStartAction; set => _preparationStartAction = value; }
        internal Action<State, Context> PreparationEndAction { get => _preparationEndAction; set => _preparationEndAction = value; }
        internal Action<State, Context> AscendingStartAction { get => _ascendingStartAction; set => _ascendingStartAction = value; }
        internal Action<State, Context> AscendingEndAction { get => _ascendingEndAction; set => _ascendingEndAction = value; }
        internal Action<State, Context> JumpEndAction { get => _jumpEndAction; set => _jumpEndAction = value; }

        public JumpLocomotion(IJumpDefinitions definitions)
        {
            if (definitions == null)
                throw new ArgumentNullException(nameof(definitions));
            _definitions = definitions;
            _startVelocity.y = Mathf.Sqrt(-2 * Physics.gravity.y * definitions.Height);
            _ascendingDurationTime = _startVelocity.y / -Physics.gravity.y;
            _state = State.OnGround;

            _preparationTimeline = new Timeline(_definitions.PreparationDuration);
            _preparationTimeline.AddPointEvent(0, _ =>
            {
                _state = State.InPreparation;
                _preparationStartAction?.Invoke(_state, _context);
            });
            _preparationTimeline.AddPointEvent(1, _ =>
            {
                _context.Velocity += Quaternion.FromToRotation(World.DefaultUp, _context.World.Up) * _startVelocity;
                _preparationEndAction?.Invoke(_state, _context);
            });

            _ascendingTimeline = new Timeline(_ascendingDurationTime);
            _ascendingTimeline.AddPointEvent(0, _ =>
            {
                _state = State.Ascending;
                _ascendingStartAction?.Invoke(_state, _context);
            });
            _ascendingTimeline.AddPointEvent(1, _ =>
            {
                _state = State.Descending;
                _ascendingEndAction?.Invoke(_state, _context);
            });



            _probes = new Vector3[]
            {
                Vector3.forward,
                Quaternion.Euler(0,30,0)*Vector3.forward,
                Quaternion.Euler(0,-30,0)*Vector3.forward,
                Quaternion.Euler(0,60,0)*Vector3.forward,
                Quaternion.Euler(0,-60,0)*Vector3.forward,
            };
        }
        public void EndJump(Context context)
        {
            if (_state == State.OnGround)
                return;

            EndJumpImpl(context);
        }

        void EndJumpImpl(Context context)
        {
            if (_preparationTimeline.IsRunning)
                _preparationTimeline.Stop();
            if (_ascendingTimeline.IsRunning)
                _ascendingTimeline.Stop();
            _state = State.OnGround;
            _jumpEndAction?.Invoke(_state, _context);
            _probeResults.TrimExcess();
        }
        void StartJumpImpl(Context context)
        {
            _context = context;
            _preparationTimeline.Start();
            _probeResults = new();
        }
        void ProbesHandle(Context context)
        {
            var pos = context.Position;
            var rotation = Quaternion.LookRotation(context.Input.HorizontalDirection);
            _probeResults.Clear();
            foreach (var p in _probes)
            {
                var probe = rotation * p;

                if (Physics.Raycast(pos, probe, out var hitInfo, 2.2f))
                {
                    _probeResults.Add(hitInfo);
                }
            }

            var v = Vector3.ProjectOnPlane(context.Velocity, context.World.Up);
            foreach (var hitInfo in _probeResults)
            {
                var normal = hitInfo.normal;
                v = Vector3.ProjectOnPlane(v, normal);
            }
            v.y = context.Velocity.y;
            context.Velocity = v;
        }
        public void OnFixedUpdate(Context context)
        {

            var input = context.Input;
            var ground = context.Ground;
            if (_state > State.InPreparation)
            {
                if (ground != null)
                    EndJumpImpl(context);
                else
                    ProbesHandle(context);
            }


            if (_state == State.InPreparation && ground == null)
                _ascendingTimeline.Start();

            if (_state == State.OnGround && ground != null && input.IsAscending)
            {
                StartJumpImpl(context);
            }

            if (_preparationTimeline.IsRunning)
                _preparationTimeline.OnUpdate(context.DeltaTime);
            if (_ascendingTimeline.IsRunning)
                _ascendingTimeline.OnUpdate(context.DeltaTime);

        }
    }

}