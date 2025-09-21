using System;
using Tests.Characters;
using Tests.States;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launchers.Animations
{
    internal class ArmAiming : ArmedLauncherAnimationStateBase
    {
        AimingHelper _aim;
        float _w;
        string _aname;
        public ArmAiming(ControllerPlayable controller, AimingHelper aimingHelper, string animationStateName, bool enabled = true) : base(controller, "aiming", 0, enabled)
        {
            _aim = aimingHelper ?? throw new ArgumentNullException(nameof(aimingHelper));
            _aname = animationStateName ?? throw new ArgumentNullException(nameof(animationStateName));
        }
        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                _aim.Enabled = value;
            }
        }
        //public IWeapon ControlledWeapon { get => _aim.ControlledWeapon; set => _aim.ControlledWeapon = value; }
        //public ITarget AimingTarget { get => _aim.Target; set => _aim.Target = value; }
        public override void OnUpdate()
        {
            base.OnUpdate();
            _aim.TargetUpdate();
        }
        public override void OnExit()
        {
            base.OnExit();
            controller.SetBool(_aname, false);
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            _w = _aim.Weight;
            controller.SetBool(_aname, true);
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            _aim.Weight = Mathf.Lerp(_w, 1, currentTransition.Timeline.NormalizedTime);
            _aim.TargetUpdate();
        }
        public override void ToNextStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionBegin(currentTransition);
            _w = _aim.Weight;
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            _aim.Weight = Mathf.Lerp(_w, 0, currentTransition.Timeline.NormalizedTime);
            _aim.TargetUpdate();
        }
    }
}
