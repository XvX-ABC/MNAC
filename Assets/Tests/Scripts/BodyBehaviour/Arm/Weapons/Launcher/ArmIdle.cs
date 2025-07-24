using System;
using Tests.States;
using UnityEngine.Assertions.Must;
using static Tests.Behaviours.Arm.Weapons.ArmedLauncherArmBehaviour;

namespace Tests.BodyBehaviour.Arm.Weapons.Launcher
{
    internal class ArmIdle : ArmedArmStateBase
    {
        BAnimator _animator;
        public ArmIdle(BAnimator animator) : base("idle", 0)
        {
            _animator = animator ?? throw new ArgumentNullException(nameof(animator));
        }
        public override void OnEnter()
        {
            _animator.IdleWeight = 1;
        }
        public override void OnTransitionWhichToNextState(IReadonlyPlayableTransition<object> currentTransition)
        {
            var time = currentTransition.Timeline.NormalizedTime;
            if (time >= 1)
                _animator.IdleWeight = 0;
        }
    }
}
