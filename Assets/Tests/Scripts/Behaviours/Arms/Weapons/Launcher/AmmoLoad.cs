using Tests.States;
using Tests.Weapons.Launcher;
using ReloadAnimator = Tests.Behaviours.Arms.Weapons.Launchers.ArmedLauncherArmBehaviour_Obsolete.ReloadAnimator;

namespace Tests.BodyBehaviour.Arm.Weapons.Launcher
{
    internal class AmmoLoad : ArmedArmStateBase
    {

        ReloadAnimator _reloadAnimator;
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
