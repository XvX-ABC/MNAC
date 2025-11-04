using System;
using Tests.Animations;
using Tests.States;
using Tests.TPhysics.Environment;
using UnityEngine;
using World = Tests.TPhysics.World;

namespace Tests.Behaviours.Arms.Weapons.Launchers.Animations
{
    internal class Idle : ArmedLauncherAnimationStateBase
    {
        Vector2 _horizontalVelocity;
        string _velocity_x;
        string _velocity_y;
        float _maxSpeed;
        float _accelerationSpeed;
        Rigidbody _rbody;
        World _world;
        IGroundDetector _groundDetector;
        public Idle(Rigidbody rbody, World world, IGroundDetector groundDetector, ControllerPlayable controller, float maxSpeed, float accelerationSpeed, string velocity_x, string velocity_y, bool enabled = true) : base(controller, "idle", 0, enabled)
        {
            _velocity_x = velocity_x ?? throw new ArgumentNullException(nameof(velocity_x));
            _velocity_y = velocity_y ?? throw new ArgumentNullException(nameof(velocity_y));
            _rbody = rbody;
            _world = world;
            _groundDetector = groundDetector;
            _maxSpeed = maxSpeed;
            _accelerationSpeed = accelerationSpeed;
        }


        public Vector2 HorizontalVelocity { get => _horizontalVelocity; set => _horizontalVelocity = value; }
        void SetVelocity()
        {
            var velocity = _rbody.velocity;
            var speed = velocity.magnitude;

            var v0 = velocity / (1 - Time.fixedDeltaTime * 0.5f * _rbody.drag);
            var a = (v0).magnitude - (velocity).magnitude;

            var v = Mathf.Clamp01(Mathf.Max(a <= 0 || _accelerationSpeed <= 0 ? 0 : a / (_accelerationSpeed * Time.fixedDeltaTime), speed / _maxSpeed));
            var rotation = _world.Rotation;
            if (_groundDetector.Grounds.Count > 0)
            {
                rotation *= Quaternion.FromToRotation(_world.Up, _groundDetector.GroundsNormal);
            }
            velocity = Quaternion.Inverse(rotation * _rbody.rotation) * (velocity.normalized * v);
            controller.SetFloat(_velocity_x, velocity.x);
            controller.SetFloat(_velocity_y, velocity.z);
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            SetVelocity();
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            SetVelocity();
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            SetVelocity();
        }
    }
}
