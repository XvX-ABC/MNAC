using System;
using Tests.Behaviours.Arm.Weapons;
using Tests.Behaviours.Arms.Weapons.Launchers;
using Tests.States;
using Tests.Weapons.Launcher;

namespace Tests.BodyBehaviour.Arm.Weapons.Launcher
{
    [Obsolete]
    internal class AmmoLoad_Obsolete : ArmedArmStateBase
    {

        ReloadAnimator_Obsolete _reloadAnimator;
        ILauncher _launcher;
        public ILauncher Launcher
        {
            set
            {
                this.timeline = value.ReloadTimeline;
                this._launcher = value;
            }
        }
        public AmmoLoad_Obsolete(ReloadAnimator_Obsolete animator) : base("ammo_load", 0)
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
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            var time = currentTransition.Timeline.NormalizedTime;
            if (time >= 0.8f)
                _reloadAnimator.Play();
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            var time = currentTransition.Timeline.NormalizedTime;
            if (time >= 0.8f)
                _reloadAnimator.Stop();
        }
    }
}
