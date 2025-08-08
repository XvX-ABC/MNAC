using System;
using Tests.BodyBehaviour.Arm.Weapons.Launcher;
using Tests.States;
using TMPro;
using UnityEngine.Assertions.Must;
using static Tests.Behaviours.Arm.Weapons.ArmedLauncherArmBehaviour;

namespace Tests.Behaviours.Arm.Weapons
{
    internal class ArmIdle : ArmedArmStateBase
    {
        ArmedLauncherArmBehaviour _behaviour;
        BAnimator _animator;
        public ArmIdle(ArmedLauncherArmBehaviour behaviour) : base("idle", 0)
        {
            _behaviour = behaviour ?? throw new ArgumentNullException(nameof(behaviour));
            _animator = behaviour.banimator ?? throw new NullReferenceException(nameof(behaviour.banimator));
        }
        public override void OnEnter()
        {
            _animator.IdleWeight = 1;
            //_behaviour.enabled = false;
            _animator.enabled = false;
        }
        public override void OnExit()
        {
        }
        public override void TransitionRunningWhichToNextState(IReadonlyPlayableTransition<object> currentTransition)
        {
            var time = currentTransition.Timeline.NormalizedTime;
            if (time >= 1)
                _animator.IdleWeight = 0;
        }
    }
}
