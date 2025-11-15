using Tests.Behaviours.Arm.Weapons;
using Tests.Weapons.Launcher;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launcher
{
    internal class AmmoLoad : ArmedArmStateBase
    {

        ILauncher _launcher;
        public ILauncher TargetLauncher
        {
            set
            {
                this.timeline = value.ReloadTimeline;
                this._launcher = value;
            }
        }
        public AmmoLoad() : base("ammo_load", 0)
        {
        }
        public override void OnEnter()
        {
            _launcher.StartReload();
        }
        public override void OnExit()
        {
            _launcher.EndReload();
        }
        //public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        //{
        //    base.FromPreviousStateTransitionBegin(currentTransition);
        //}
        //public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        //{
        //    base.FromPreviousStateTransitionRunning(currentTransition);
        //    var time = currentTransition.Timeline.NormalizedTime;
        //    if (time >= 0.8f)
        //        _reloadAnimator.Play();
        //}
        //public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        //{
        //    base.ToNextStateTransitionRunning(currentTransition);
        //    var time = currentTransition.Timeline.NormalizedTime;
        //    if (time >= 0.8f)
        //        _reloadAnimator.Stop();
        //}
    }
}
