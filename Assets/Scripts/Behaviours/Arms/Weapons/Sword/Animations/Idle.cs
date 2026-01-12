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

        float _w0;
        float _w1;
        ControllerPlayable _baseController;
        WholeBodyControllerPlayable _wholeBodyController;
        float baseWeight { get => _baseController.OutputSetting.Weight; set => _baseController.OutputSetting.Weight = value; }
        float wholeBodyWeight { get => _wholeBodyController.OutputSetting.Weight; set => _wholeBodyController.OutputSetting.Weight = value; }
        public Idle(ArmControllerPlayable armController, ControllerPlayable baseController, WholeBodyControllerPlayable wholeBodyController, LocomotionCore locomotionCore, float maxSpeed, float accelerationSpeed, bool enabled = true) : base(armController, "idle", 0, enabled)
        {
            _baseController = baseController ?? throw new ArgumentNullException(nameof(baseController));
            _wholeBodyController = wholeBodyController ?? throw new ArgumentNullException(nameof(wholeBodyController));
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

        void RecordWeights()
        {
            _w0 = baseWeight;
            _w1 = wholeBodyWeight;
        }
        void UpdateWeights(ushort exceptedWeight, float t)
        {
            baseWeight = Mathf.Lerp(_w0, 1 - exceptedWeight, t);
            wholeBodyWeight = Mathf.Lerp(_w1, exceptedWeight, t);
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
        public override void OnEnter()
        {
            base.OnEnter();
            UpdateWeights(0, 1);
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            _locomotion.Enabled = true;
            RecordWeights();
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            UpdateWeights(0, currentTransition.Timeline.NormalizedTime);
        }
        public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            _locomotion.Enabled = false;
            base.ToNextStateTransitionEnd(currentTransition);
        }
    }
}
