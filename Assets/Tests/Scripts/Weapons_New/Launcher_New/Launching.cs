using Tests.Utilities.Timeline;
using Tests.Utilities.Timeline.Events.Point;
using UnityEngine;

namespace Tests.Weapons_New.Launcher
{
    internal class Launching : LauncherState
    {
        internal Timeline_V2 intervalTimeline;
        float _interval;
        bool _launchWhenEnter;
        public Launching(Launcher launcher, string name, float interval, bool launchWhenEnter = false, bool enabled = true) : base(launcher, $"{name}_launching", 0, enabled)
        {
            _interval = Mathf.Max(0, interval);
            _launchWhenEnter = launchWhenEnter;
            if (_interval > 0)
            {
                intervalTimeline = new(interval, true);
                intervalTimeline.AddPointEvent(1, _ =>
                {
                    launcher.Launch();
                });
            }
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (_interval > 0)
            {
                intervalTimeline.OnUpdate(Time.deltaTime);
            }
        }
        public override void OnEnter()
        {
            base.OnEnter();
            if (_interval == 0)
                launcher.Launch();
            else
            {
                if (_launchWhenEnter)
                {
                    intervalTimeline.Reset();
                    intervalTimeline.SetTime(_interval);
                    intervalTimeline.Start();
                }
                else
                {
                    intervalTimeline.Restart();
                }
            }
        }
        public override void OnExit()
        {
            if (_interval > 0)
                intervalTimeline.End();
            base.OnExit();
        }
    }
}
