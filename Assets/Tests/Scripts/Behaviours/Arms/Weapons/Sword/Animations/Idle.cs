using System;
using System.Linq;
using Tests.Animations;
using Tests.States;
using Tests.TPhysics.Locomotion;
using UnityEngine;
using UnityEngine.Android;

namespace Tests.Behaviours.Arms.Weapons.Sword.Animations
{
    internal class Idle : ArmedSwordAnimationStateBase
    {
        float _maxSpeed;
        float _accelerationSpeed;
        LocomotionCore _locomotionCore;
        IdleArmAnimationLocomotion _locomotion;
        ArmControllerPlayable _armController;
        public Idle(ArmControllerPlayable armController, LocomotionCore locomotionCore, float maxSpeed, float accelerationSpeed, bool enabled = true) : base(armController, "idle", 0, enabled)
        {
            _maxSpeed = Mathf.Max(0, maxSpeed);
            _accelerationSpeed = Mathf.Max(0, accelerationSpeed);
            _armController = armController;



            _locomotionCore = locomotionCore ?? throw new ArgumentNullException(nameof(locomotionCore));
            _locomotion = new IdleArmAnimationLocomotion(this);
            //_locomotionCore.AddModule(_locomotion);
            _locomotionCore.EvaluationModules = _locomotionCore.EvaluationModules.Append(_locomotion).ToArray();
        }
        internal Idle(ArmControllerPlayable armController, LocomotionCore locomotionCore, IdleArmAnimationLocomotion locomotion, float maxSpeed, float accelerationSpeed, bool enabled = true) : base(armController, "idle", 0, enabled)
        {
            _maxSpeed = Mathf.Max(0, _maxSpeed);
            _accelerationSpeed = Mathf.Max(0, _accelerationSpeed);
            _armController = armController;


            _locomotionCore = locomotionCore ?? throw new ArgumentNullException(nameof(locomotionCore));
            _locomotion = locomotion ?? throw new ArgumentNullException(nameof(locomotion));
            _locomotionCore.EvaluationModules = _locomotionCore.EvaluationModules.Append(_locomotion).ToArray();
        }
        internal void SetVelocity(Context context)
        {
            var rbody = context.Rbody;
            var world = context.World;
            var velocity = context.CurrentVelocity;
            var speed = velocity.magnitude;
            var dtime = Time.fixedDeltaTime;
            var gg = context.GroundDetector;

            var v0 = velocity / (1 - dtime * 0.5f * rbody.drag);
            var a = v0.magnitude - speed;

            var v = Mathf.Clamp01(Mathf.Max(a <= 0 || _accelerationSpeed <= 0 ? 0 : a / (_accelerationSpeed * dtime), speed / _maxSpeed));
            var rotation = world.Rotation;
            if (gg.Grounds.Count > 0)
            {
                rotation *= Quaternion.FromToRotation(world.Up, gg.GroundsNormal);
            }
            velocity = Quaternion.Inverse(rotation * context.CurrentRotation) * (velocity.normalized * v);
            //controller.SetFloat(_velocityName_x, velocity.x);
            //controller.SetFloat(_velocityName_y, velocity.z);
            _armController.SetVelocity(new Vector2(velocity.x, velocity.z));
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            _locomotion.Enabled = true;
        }
        public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            _locomotion.Enabled = false;
            base.ToNextStateTransitionEnd(currentTransition);
        }
    }
}
