using MNAC.Utilities.Timeline.Events.Point;
using UnityEngine;

namespace MNAC.Weapons.Launcher
{
    internal class Reload : LauncherState
    {
        float _changeProportion;
        public Reload(Launcher launcher, string name, float changeProportion, float duration = 0, bool enabled = true) : base(launcher, $"{name}_reload", duration, enabled)
        {
            _changeProportion = Mathf.Clamp01(_changeProportion);
            timeline.AddPointEvent(changeProportion, _ =>
            {
                launcher.Reload();
            });
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
        }
    }
}
