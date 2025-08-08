using Assets.Scripts.Utilities.Timeline;
using Tests.States;
using Tests.Weapons.Launcher;
using UnityEngine;
using static Tests.Behaviours.Arm.Weapons.ArmedLauncherArmBehaviour;
using ReloadAnimator = Tests.Behaviours.Arm.Weapons.ArmedLauncherArmBehaviour.ReloadAnimator;

namespace Tests.BodyBehaviour.Arm.Weapons.Launcher
{
    internal class AmmoLoad : ArmedArmStateBase
    {

        ReloadAnimator _reloadAnimator;
        internal BAnimator animator;
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
            _reloadAnimator = animator;
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
        public override void TransitionBeginWhichOfPreviousState(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.TransitionBeginWhichOfPreviousState(currentTransition);
            if (animator != null)
                animator.enabled = true;
        }
        public override void TransitionRunningWhichOfPreviousState(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.TransitionRunningWhichOfPreviousState(currentTransition);
            var time = currentTransition.Timeline.NormalizedTime;
            if (time >= 0.8f)
                _reloadAnimator.Play();
        }
        public override void TransitionRunningWhichToNextState(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.TransitionRunningWhichToNextState(currentTransition);
            var time = currentTransition.Timeline.NormalizedTime;
            if (time >= 0.8f)
                _reloadAnimator.Stop();
        }
    }
}
