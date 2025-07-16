using Assets.Scripts.Utilities.Timeline;
using Tests.States;
using Tests.Weapons.Launcher;
using UnityEngine;
using ReloadAnimator = Tests.Behaviours.Arm.Weapons.ArmedLauncherArmBehaviour.ReloadAnimator;

namespace Tests.BodyBehaviour.Arm.Weapons.Launcher
{
    internal class AmmoLoad : ArmedArmStateBase
    {

        ReloadAnimator _animator;
        ILauncher _launcher;
        public ILauncher Launcher
        {
            set
            {
                this.timeline = value.ReloadTimeline;
                this._launcher = value;
            }
        }
        public AmmoLoad(ReloadAnimator animator) : base("ammo_load", 0)
        {
            _animator = animator;
        }
        public override void OnEnter()
        {
            //timeline.Start();
            _launcher.StartReload();
        }
        public override void OnExit()
        {
            _launcher.EndReload();
        }
        public override void OnTransitionWhichOfPreviousState(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.OnTransitionWhichOfPreviousState(currentTransition);
            var time = currentTransition.Timeline.NormalizedTime;
            if (time <= 0)
                _animator.Play();
        }
        public override void OnTransitionWhichToNextState(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.OnTransitionWhichToNextState(currentTransition);
            var time = currentTransition.Timeline.NormalizedTime;
            if (time >= 1)
                _animator.Stop();
        }
    }
}
