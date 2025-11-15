using System;
using Tests.Behaviours.Arm.Weapons;
using Tests.Behaviours.Arms.Weapons.Launcher;
using Tests.States;

namespace Tests.Behaviours.Arms.Weapons
{
    internal class ArmIdle : ArmedArmStateBase
    {
        ArmedLauncherArmAnimator_Obsolete _animator;
        public ArmIdle(ArmedLauncherArmAnimator_Obsolete animator) : base("idle", 0)
        {
            _animator = animator ?? throw new ArgumentNullException(nameof(animator));
        }
        public override void OnEnter()
        {
            _animator.IdleWeight = 1;
            //_behaviour.enabled = false;
        }
        public override void OnExit()
        {
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            var time = currentTransition.Timeline.NormalizedTime;
            if (time >= 1)
                _animator.IdleWeight = 0;
        }
    }
}
