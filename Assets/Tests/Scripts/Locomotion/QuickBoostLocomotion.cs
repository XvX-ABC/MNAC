using System;
using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using Assets.Scripts.Utilities.Timeline.Event.Range;
using Locomotion;
using UnityEngine;
namespace Tests.Locomotion
{
    class QuickBoostLocomotion : IModule
    {
        IQuickBoostDefinitions _definitions;
        IBaseDefinitions _baseDefinitions;
        JumpLocomotion_New _jumpLocomotion;
        Timeline _timeline;
        Context _context;
        Vector3 _velocity;
        float _lastTime;
        Action<Context> _startAction;
        Action<Context> _endAction;
        public IQuickBoostDefinitions Definitions { get => _definitions; }
        public Action<Context> StartAction { get => _startAction; set => _startAction = value; }
        public Action<Context> EndAction { get => _endAction; set => _endAction = value; }
        public ITimeline Timeline => _timeline;
        public QuickBoostLocomotion(IBaseDefinitions baseDefinitions, IQuickBoostDefinitions definitions, JumpLocomotion_New jumpLocomotion)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            //_timeline = new(_definition.Duration, false, new Boosting(0f, 1f, this), new EndBoostEvent(1f, this));
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
            var direction = context.World.Input.HorizontalDirection;
            if (direction == Vector3.zero)
                return;
            var speed = _baseDefinitions.Speed * _definitions.Power;
            var expectedVelocity = direction * speed;
            _velocity = expectedVelocity;
            _context = context;
            _timeline.Start();
            _startAction?.Invoke(context);
        }
        public void EndBoost(Context context)
        {
            if (!_timeline.isRunning)
                return;
            Debug.Log("End boost");
            _timeline.Stop();
            _lastTime = Time.unscaledTime;
            _velocity = Vector3.zero;
            _endAction?.Invoke(context);
        }
        public Vector3 CalculateVelocity(Vector3 direction, Vector3 currentVelocity)
        {
            direction = direction.normalized;
            var velocity = direction * _definitions.Velocity;
            velocity.y = currentVelocity.y;
            return velocity;
        }
        public void OnFixedUpdate(Context context)
        {
            _context = context;
            var input = context.Input;
            if (input.IsBoosting && !_timeline.IsRunning)
            {
                if (_jumpLocomotion.CurrentState > JumpLocomotion_New.State.OnGround)
                    _jumpLocomotion.EndJump(context);

                StartBoost(context);
            }
            if (_timeline.IsRunning)
                _timeline.OnUpdate(context.DeltaTime);

        }
    }
}