using System;
using Locomotion;
using Tests.Environment;
using Tests.Utilities.Timeline;
using UnityEngine;
using Tests.Utilities.Timeline.Events.Point;
using Tests.Utilities.Timeline.Events.Range;
namespace Tests.Locomotion_Obsolete
{
    class BoostingLocomotion : IModule
    {
        IBoostingDefinitions _definitions;
        IBaseDefinitions _baseDefinitions;
        JumpLocomotion _jumpLocomotion;
        Timeline _timeline;
        Context _context;
        Vector3 _velocity;
        float _lastTime;
        Action<Context> _startAction;
        Action<Context> _endAction;
        public IBoostingDefinitions Definitions { get => _definitions; }
        public Action<Context> StartAction { get => _startAction; set => _startAction = value; }
        public Action<Context> EndAction { get => _endAction; set => _endAction = value; }
        public ITimeline Timeline => _timeline;
        public BoostingLocomotion(IBaseDefinitions baseDefinitions, IBoostingDefinitions definitions, JumpLocomotion jumpLocomotion)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _timeline = new(_definitions.Duration);
            _timeline.AddRangeEvent(0f, 1f, _ => _context.Velocity = _velocity);
            _timeline.AddPointEvent(1f, _ => EndBoost(_context));
            _jumpLocomotion = jumpLocomotion;
            _baseDefinitions = baseDefinitions;
        }
        public void StartBoost(Context context)
        {

            if (_timeline.IsRunning)
                return;
            var currentTime = Time.unscaledTime;
            if (Mathf.Abs(currentTime - _lastTime) < _definitions.Interval)
                return;
            var direction = context.World.Input.HorizontalVector;
            if (direction == Vector3.zero)
                return;
            var speed = _baseDefinitions.MaxSpeed * _definitions.Power;
            var expectedVelocity = direction * speed;
            _velocity = expectedVelocity;
            _context = context;
            _timeline.Restart();
            _startAction?.Invoke(context);
        }
        public void EndBoost(Context context)
        {
            if (!_timeline.IsRunning)
                return;
            _timeline.Pause();
            _lastTime = Time.unscaledTime;
            _velocity = Vector3.zero;
            _endAction?.Invoke(context);
        }
        public void OnFixedUpdate(Context context)
        {
            _context = context;
            var input = context.Input;
            if (input.Boost && !_timeline.IsRunning)
            {
                if (_jumpLocomotion.CurrentState > JumpLocomotion.State.OnGround)
                    _jumpLocomotion.EndJump(context);

                StartBoost(context);
            }
            if (_timeline.IsRunning)
                _timeline.OnUpdate(context.DeltaTime);

        }
    }
}