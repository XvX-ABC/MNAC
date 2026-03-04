using MNAC.Utilities.Timeline;
using UnityEngine;

namespace MNAC.Weapons.Launcher
{
    internal class DelayLaunch : LauncherState
    {
        public DelayLaunch(Launcher launcher, string name, Vector2 range, bool enabled = true) : base(launcher, $"{name}_delay_launch", 0, enabled)
        {
            timeline = new RandomLengthTimeline_V1(range);
        }
        protected override ITimeline NewTimeline(float duration)
        {
            return null;
        }
    }
}
