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
        ISphericalObjsTrigger _targetTrigger;
        ITargetLocker _targetLocker;

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

        public Boosting(BoostingLocomotion locomotion, LocomotionCore locomotionCore, ITargetLocker targetLocker, IBaseInput input, float duration) : base("boosting", 0)
        {
            _locomotion = locomotion ?? throw new ArgumentNullException(nameof(locomotion));
            _locomotionCore = locomotionCore ?? throw new ArgumentNullException(nameof(locomotionCore));
            TargetLocker = targetLocker;
            timeline = new Timeline_V1(duration);

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
            var f = _locomotionCore.Context.Forward;
            _locomotion.DirectionVector = f;
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
