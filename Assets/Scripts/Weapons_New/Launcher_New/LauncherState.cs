using System;
using Tests.States;
using Tests.Utilities.Timeline;
using UnityEngine;

namespace Tests.Weapons_New.Launcher
{

    internal class LauncherState : WithCallbackPlayableState<object>
    {
        protected Launcher launcher;
        public LauncherState(Launcher launcher, string name, float duration = 0, bool enabled = true) : base($"launcher_{name}", duration, enabled)
        {
            this.launcher = launcher ?? throw new ArgumentNullException(nameof(launcher));
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
    }
}
