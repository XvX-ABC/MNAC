#define LOCOMOTION_JUMP_DIRECTION_KEEP
using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using Assets.Scripts.Utilities.Timeline.Event.Range;
using Locomotion;
using System;
using System.Data;
using UnityEngine;
using LState = Tests.Locomotion.State;
namespace Tests.Locomotion
{


    class JumpLocomotion : IModule
    {
        public enum State
        {
            Idle,
            Preparating,
            Ascending,
            Descending
        }
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
                _locomotion._context.State = LState.Ascending;
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
                _locomotion._currentVelocity.y = _locomotion.CalculateVelocity(_time).y;
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
                _locomotion._context.State = LState.Descending;
            }
        }
        Vector3 _startVelocity;
        Vector3 _currentVelocity;
        float _ascendingDuration;
        IJumpDefines _defines;
        //State __currentState { get => _context.State; set => _context.State = value; }
        State _currentState;
        Timeline _timeline;
        Context _context;

        public State CurrentState
        {
            get => _currentState;
        }
        public float AscendingDuration { get => _ascendingDuration; }
        public IJumpDefines Defines { get => _defines; }
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
        public void StartJump()
        {
            _currentVelocity = _context.Velocity;
            _currentState = State.Preparating;
            _timeline.Start();
        }
        public void EndJump()
        {
            _currentState = State.Idle;
            _currentVelocity = Vector3.zero;
            _timeline.Stop();
        }
        Vector3 CalculateVelocity(float time)
        {
            return _startVelocity + Physics.gravity * time;
        }
        //StringBuilder builder = new StringBuilder();

        public void OnUpdate(Context context)
        {
            var input = context.Input;
            var ground = context.Ground;
            var state = context.State;
            if (_context != context)
                _context = context;
            if (_currentState == State.Idle && state == LState.OnGround && input.IsAscending)
                StartJump();
            else if (_currentState == State.Descending && ground.Touched)
            {
                EndJump();
                return;
            }


            if (_timeline.IsRunning)
                _timeline.OnUpdate(Time.fixedDeltaTime);


#if LOCOMOTION_JUMP_DIRECTION_KEEP
            if (_currentState == State.Ascending)
                context.Velocity = _currentVelocity;
            else if (_currentState == State.Descending)
            {
                //if (ground.Touched)
                //    EndJump();
                //else
                //{
                //    var currentVelocity = new Vector3(_currentVelocity.x, _context.Velocity.y, _currentVelocity.z);
                //    _context.Velocity = currentVelocity;
                //}
                var currentVelocity = new Vector3(_currentVelocity.x, _context.Velocity.y, _currentVelocity.z);
                _context.Velocity = currentVelocity;
            }
#else
            else if (__currentState == State.Ascending)
            {
                var velocity = context.Velocity;
                velocity.y = _currentVelocity.y;
                context.Velocity = velocity;
            }
            else if (__currentState == State.Descending && ground.Touched)
                EndJump();
#endif
          

        }
    }

}