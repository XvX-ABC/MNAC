using System;
using Tests.TPhysics;
using Tests.TPhysics.Environment;
using UnityEngine;

namespace Tests.Characters.Locomotion.Animations
{
    internal class MovementAnimator
    {
        float _weight;
        bool _inAir;
        float maxSpeed;
        float _accelerationSpeed;
        Vector3 _velocity;
        Rigidbody rbody;
        World world;
        IGroundDetector groundDetector;
        ILocomotionAnimatorDefinitions definitions;
        internal ControllerPlayable controller;
        public MovementAnimator(ILocomotionAnimatorDefinitions definitions, float maxSpeed, Rigidbody rbody, World world, IGroundDetector groundDetector, ControllerPlayable controller)
        {
            this.definitions = definitions;
            this.maxSpeed = maxSpeed;
            this.rbody = rbody;
            this.world = world;
            this.groundDetector = groundDetector;
            this.controller = controller;
        }
        public MovementAnimator(ILocomotionAnimatorDefinitions definitions, float maxSpeed, float accelerationSpeed, Rigidbody rbody, World world, IGroundDetector groundDetector, ControllerPlayable controller)
        {
            this.definitions = definitions;
            this.maxSpeed = maxSpeed;
            this._accelerationSpeed = accelerationSpeed;
            this.rbody = rbody;
            this.world = world;
            this.groundDetector = groundDetector;
            this.controller = controller;
        }
        public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
        [Obsolete]
        public float Weight { get => _weight; set => _weight = Mathf.Clamp01(value); }
        [Obsolete]
        public bool InAir
        {
            get => _inAir;
            set
            {
                _inAir = value;
                //controller.SetBool(definitions.InAir, _inAir);
            }
        }
        public void Update()
        {
            var velocity = rbody.velocity;
            var speed = velocity.magnitude;

            var v0 = velocity / (1 - Time.fixedDeltaTime * 0.5f * rbody.drag);
            var a = (v0).magnitude - (velocity).magnitude;

            var v = Mathf.Clamp01(Mathf.Max(a <= 0 || _accelerationSpeed <= 0 ? 0 : a / (_accelerationSpeed * Time.fixedDeltaTime), speed / maxSpeed));
            var rotation = world.rotation;
            if (groundDetector.Grounds.Count > 0)
            {
                rotation *= Quaternion.FromToRotation(world.Up, groundDetector.GroundsNormal);
            }
            velocity = Quaternion.Inverse(rotation * rbody.rotation) * (velocity.normalized * v);
            controller.SetFloat(definitions.Velocity_X, velocity.x);
            controller.SetFloat(definitions.Velocity_Y, velocity.z);
        }
    }
}
