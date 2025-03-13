using System;
using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using Assets.Scripts.Utilities.Timeline.Event.Range;
using Locomotion;
using UnityEngine;
using JState = Tests.Locomotion.JumpLocomotion.State;
namespace Tests.Locomotion
{
    class QuickBoostLocomotion : IModule
    {
        //class Boosting : RangeEvent
        //{
        //    QuickBoostLocomotion _locomotion;
        //    public Boosting(float triggeredProportion, float durationProportion, QuickBoostLocomotion locomotion) : base(durationProportion, triggeredProportion)
        //    {
        //        _locomotion = locomotion;
        //    }
        //    public override void Execute(TimelineContext _)
        //    {
        //        Debug.Log("Boosting: " + _locomotion._velocity);
        //        _locomotion._context.Velocity = _locomotion._velocity;
        //    }
        //}
        //class EndBoostEvent : PointEvent
        //{
        //    QuickBoostLocomotion _locomotion;

        //    public EndBoostEvent(float triggeredProportion, QuickBoostLocomotion locomotion) : base(triggeredProportion)
        //    {
        //        _locomotion = locomotion;
        //    }

        //    public override void Execute(TimelineContext context)
        //    {
        //        _locomotion.EndBoost();
        //    }
        //}
        IQuickBoostDefinitions _definition;
        JumpLocomotion _jumpLocomotion;
        Timeline _timeline;
        Context _context;
        Vector3 _velocity;
        float _lastTime;
        public QuickBoostLocomotion(IQuickBoostDefinitions definition, JumpLocomotion jumpLocomotion)
        {
            _definition = definition ?? throw new ArgumentNullException(nameof(definition));
            //_timeline = new(_definition.Duration, false, new Boosting(0f, 1f, this), new EndBoostEvent(1f, this));
            _timeline = new(_definition.Duration);
            _timeline.AddRangeEvent(0f, 1f, _ => _context.Velocity = _velocity);
            _timeline.AddPointEvent(1f, _ => EndBoost());
            _jumpLocomotion = jumpLocomotion;
        }
        public void StartBoost()
        {
            if (_timeline.isRunning)
                return;

            var currentTime = Time.unscaledTime;
            if (Mathf.Abs(currentTime - _lastTime) < _definition.Interval)
                return;


            var context = _context;
            var direction = context.Input.HorizontalDirection;
            var velocity = context.Velocity;

            _velocity = CalculateVelocity(direction, velocity);

            _timeline.Start();

        }
        public void EndBoost()
        {
            if (!_timeline.isRunning)
                return;
            _timeline.Stop();
            _lastTime = Time.unscaledTime;
            _velocity = Vector3.zero;
        }
        public Vector3 CalculateVelocity(Vector3 direction, Vector3 currentVelocity)
        {
            direction = direction.normalized;
            var velocity = direction * _definition.Velocity;
            velocity.y = currentVelocity.y;
            return velocity;
        }
        public void OnUpdate(Context context)
        {
            _context = context;
            var input = context.Input;
            if (input.IsBoosting)
            {
                if (_jumpLocomotion.CurrentState > JState.Idle && _jumpLocomotion.CurrentState <= JState.Ascending)
                    _jumpLocomotion.EndJump();

                StartBoost();
            }
            if (_timeline.IsRunning)
                _timeline.OnUpdate(context.DeltaTime);

        }
    }
}