using System;
using Tests.Behaviours.Arm.Weapons;
using Tests.Input;
using Tests.Interaction;
using Tests.States;
using Tests.TPhysics.Locomotion;
using Tests.Utilities.TransformHelper;
using UnityEngine;
using Utilities.Timeline;

namespace Tests.Behaviours.Arms.Weapons.Sword
{
    internal class Boosting : ArmedArmStateBase
    {
        enum LifeCycle
        {
            Ready,
            FPS_Transition_Running,
            Entered,
            Update,
            Exited,
            NPS_Transition_Running,
        }
        LocomotionCore _locomotionCore;
        BoostingLocomotion _locomotion;
        RotationByScreen _rotationHelper;
        ITargetsCatcher _targetsCatcher;
        IInput _input;

        LifeCycle _life;


        public ITargetsCatcher TargetsCatcher
        {
            get => _targetsCatcher;
            set
            {
                _targetsCatcher = value;
                UpdateTargetsCatcherState();
            }
        }
        void UpdateTargetsCatcherState()
        {
            if (_targetsCatcher != null)
                _targetsCatcher.Enabled = _life > LifeCycle.Ready && _life < LifeCycle.Exited;
        }

        public Boosting(BoostingLocomotion locomotion, LocomotionCore locomotionCore, Camera camera, IInput input, float duration) : base("boosting", 0)
        {
            _locomotion = locomotion ?? throw new ArgumentNullException(nameof(locomotion));
            _locomotionCore = locomotionCore ?? throw new ArgumentNullException(nameof(locomotionCore));
            timeline = new Timeline_V1(duration);
            _rotationHelper = new();
            _rotationHelper.Camera = camera;
            _input = input ?? throw new ArgumentNullException(nameof(input));

            locomotionCore.AddModule(locomotion);

            _life = LifeCycle.Ready;
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            _locomotionCore.EnableModule(_locomotion);
            _life = LifeCycle.FPS_Transition_Running;
            UpdateTargetsCatcherState();
        }
        public override void ToNextStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionBegin(currentTransition);
            _life = LifeCycle.NPS_Transition_Running;
        }
        public override void OnEnter()
        {
            base.OnEnter();
            timeline.Restart();


            _life = LifeCycle.Entered;
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            //_locomotion.HorizontalVectorWhenNoTarget = _locomotionCore.Context.CurrentRotation * Vector3.forward;
            var pos = _locomotionCore.Context.CurrentPosition;
            _rotationHelper.OriginalPos = _rotationHelper.Camera.WorldToScreenPoint(pos);
            _rotationHelper.TargetPos = _input.MousePosition;
            _locomotion.HorizontalVector = _rotationHelper.Calculate() * Vector3.forward;
            timeline.OnUpdate(Time.deltaTime);


            _life = LifeCycle.Update;
        }
        public override void OnExit()
        {
            timeline.Pause();
            _locomotionCore.DisableModule(_locomotion);
            base.OnExit();


            _life = LifeCycle.Exited;
            UpdateTargetsCatcherState();
        }

    }
}
