using System;
using Tests.Behaviours.Arms.Weapons.Launchers;
using Tests.BodyBehaviour.Arm.Weapons.Launcher;
using Tests.States;
using TMPro;
using UnityEngine.Assertions.Must;
using static Tests.Behaviours.Arms.Weapons.Launchers.ArmedLauncherArmBehaviour_Obsolete;

namespace Tests.Behaviours.Arms.Weapons
{
    internal class ArmIdle : ArmedArmStateBase
    {
        ArmedLauncherArmAnimator _animator;
        public ArmIdle(ArmedLauncherArmAnimator animator) : base("idle", 0)
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
