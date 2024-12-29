using Assets.Scripts.Utilities.Timeline;
using global::Locomotion;
using Locomotion;
using System;
using UnityEngine;
using static Tests.Locomotion.IAirModule;

namespace Tests.Locomotion
{


    class JumpLocomotion : IAirModule
    {

        class PrepareCompleted : PointEvent
        {
            JumpLocomotion _locomotion;

            public PrepareCompleted(float triggerProportion, JumpLocomotion locomotion) : base(triggerProportion)
            {
                _locomotion = locomotion;
            }


            public override void Execute(TimelineContext context)
            {
                _locomotion._currentState = State.Ascending;
            }
        }
        class Ascending : RangeEvent
        {
            JumpLocomotion _locomotion;
            float _time;

            public Ascending(float triggeredProportion, float durationProportion, JumpLocomotion locomotion) : base(durationProportion, triggeredProportion)
            {
                _locomotion = locomotion;
            }

            public override void Execute(TimelineContext context)
            {
                //_locomotion._currentVelocity = _locomotion.CalculateVelocityInAscendingStage(_time);
                _locomotion._currentVelocity = _locomotion.CalculateVelocity(_time);
                _time += context.DeltaTime;
            }
            public override void Reset()
            {
                _time = 0;
            }
        }
        class AscendingEnd : PointEvent
        {
            JumpLocomotion _locomotion;

            public AscendingEnd(float triggeredProportion, JumpLocomotion locomotion) : base(triggeredProportion)
            {
                _locomotion = locomotion;
            }

            public override void Execute(TimelineContext context)
            {
                _locomotion._currentState = State.Descending;
            }
        }
        Vector3 _startVelocity;
        Vector3 _currentVelocity;
        float _ascendingDuration;
        IJumpDefines _defines;
        State _currentState;
        Timeline _timeline;

        public State CurrentState { get => _currentState; set => _currentState = value; }

        public JumpLocomotion(IJumpDefines defines)
        {
            _defines = defines ?? throw new ArgumentNullException(nameof(defines));
            Initialize();
        }

        void Initialize()
        {
            _startVelocity.y = Mathf.Sqrt(-2 * Physics.gravity.y * _defines.Height);
            _ascendingDuration = _startVelocity.y / -Physics.gravity.y;

            var t0 = _defines.PreparationDuration / _ascendingDuration;
            var t1 = (1 - t0);
            _timeline = new(_ascendingDuration + _defines.PreparationDuration, false, new PrepareCompleted(t0, this), new Ascending(t0, t1, this), new AscendingEnd(1, this));

        }
        void StartJump()
        {
            if (_currentState > State.OnGround)
                return;
            _currentVelocity = Vector3.zero;
            _currentState = State.Preparing;
            _timeline.Start();
        }
        void EndJump()
        {
            if (_currentState == State.OnGround)
                return;
            _currentState = State.OnGround;
            _timeline.Stop();
        }
        Vector3 CalculateVelocity(float time)
        {
            return _startVelocity + Physics.gravity * time;
        }
        public void Update(Context context)
        {
            var input = context.Input;
            var ground = context.Ground;
            if (_currentState == State.OnGround && input.IsAscending)
                StartJump();
            else if (_currentState == State.Descending && ground.Touched)
                EndJump();
            else
            {
                _timeline.OnUpdate(Time.fixedDeltaTime);
            }
            context.Velocity += _currentVelocity;
        }
    }

}