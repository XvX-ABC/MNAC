using MNAC.Behaviours.Arm.Weapons;
using MNAC.States;
using MNAC.Utilities.Timeline;
using MNAC.Weapons.Launcher;
using UnityEngine;

namespace MNAC.Behaviours.Arms.Weapons.Launcher
{
    internal class AmmoLoad : ArmedArmStateBase
    {

        ILauncher _launcher;
        StateLifeCycleWatcher<object> _lifeCycleWatcher;
        float timeer;
        public ILauncher TargetLauncher
        {
            set
            {
                //this.timeline = value.ReloadTimeline;

                if (_launcher != null)
                    _launcher.ReloadTrigger -= ReloadTrigger;

                if (value != null)
                {
                    value.ReloadTrigger += ReloadTrigger;
                    timeline.UpdateLength(value.ReloadTimeline.Length);
                }
                this._launcher = value;
            }
        }
        public AmmoLoad() : base("ammo_load", 0)
        {
            _lifeCycleWatcher = new(this);
        }
        protected override ITimeline NewTimeline(float duration)
        {
            return new Timeline(duration);
        }
        public override void OnEnter()
        {
            base.OnEnter();
            timeline.Restart();
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            timeline.OnUpdate(Time.deltaTime);
        }
        public override void OnExit()
        {
            timeline.End();
            base.OnExit();
        }
        bool ReloadTrigger()
        {
            return _lifeCycleWatcher.CurrentState == LifeCycleState.Entered || _lifeCycleWatcher.CurrentState == LifeCycleState.Update;
        }
        //public override void OnEnter()
        //{
        //    _launcher.StartReload();
        //}
        //public override void OnExit()
        //{
        //    _launcher.EndReload();
        //}
    }
}
