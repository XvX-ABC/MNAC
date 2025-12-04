using System;
using Tests.Behaviours.Arm.Weapons;
using Tests.Behaviours.Input;
using Tests.Interaction;
using Tests.States;
using Tests.TPhysics.Locomotion;
using Tests.Utilities.Timeline;
using Tests.Utilities.TransformHelper;
using UnityEngine;

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
        Camera _camera;
        ISphericalObjsTrigger _targetTrigger;
        ITargetLocker _targetLocker;
        IBaseInput _baseInput;

        LifeCycle _life;

        public ISphericalObjsTrigger TargetsTrigger
        {
            get => _targetTrigger;
            set
            {
                _targetTrigger = value;
                UpdateTargetsCatcherState();
            }
        }
        public ITargetLocker TargetLocker
        {
            get => _targetLocker;
            set
            {
                _targetLocker = value ?? throw new ArgumentNullException(nameof(_targetLocker));
            }
        }
        void UpdateTargetsCatcherState()
        {
            if (_targetTrigger != null)
                _targetTrigger.Enabled = _life > LifeCycle.Ready && _life < LifeCycle.Exited;
        }

        public Boosting(BoostingLocomotion locomotion, LocomotionCore locomotionCore, Camera camera, ITargetLocker targetLocker, IBaseInput input, float duration) : base("boosting", 0)
        {
            _locomotion = locomotion ?? throw new ArgumentNullException(nameof(locomotion));
            _locomotionCore = locomotionCore ?? throw new ArgumentNullException(nameof(locomotionCore));
            TargetLocker = targetLocker;
            timeline = new Timeline_V1(duration);
            _rotationHelper = new();
            _rotationHelper.Camera = camera;
            _baseInput = input ?? throw new ArgumentNullException(nameof(input));
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));

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
            var pos = _locomotionCore.Context.CurrentPosition;
            _rotationHelper.OriginalPos = _rotationHelper.Camera.WorldToScreenPoint(pos);
            _rotationHelper.TargetPos = _targetLocker.MainLockTarget == null ? _baseInput.MousePosition : _camera.WorldToScreenPoint(_targetLocker.MainLockTarget.Position);
            _locomotion.DirectionVector = _targetTrigger.CaughtItems.Count <= 0 ? _locomotionCore.Context.CurrentRotation * Vector3.forward : Vector3.zero;
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
