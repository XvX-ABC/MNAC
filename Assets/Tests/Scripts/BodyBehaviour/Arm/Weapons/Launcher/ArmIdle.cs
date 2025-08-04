using System;
using Tests.Behaviours.Arm.Weapons;
using Tests.States;
using TMPro;
using UnityEngine.Assertions.Must;
using static Tests.Behaviours.Arm.Weapons.ArmedLauncherArmBehaviour;

namespace Tests.BodyBehaviour.Arm.Weapons.Launcher
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
            _animator.state = 0;
            //_animator.enabled = false;
            //_behaviour.enabled = false;
        }
        public override void OnExit()
        {
            _animator.state = 1;
            //_behaviour.enabled = true;
        }
        public override void OnTransitionWhichToNextState(IReadonlyPlayableTransition<object> currentTransition)
        {
            var time = currentTransition.Timeline.NormalizedTime;
            if (time >= 1)
                _animator.IdleWeight = 0;
        }
    }
}
