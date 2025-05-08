using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Range;
using System;
using Tests.Input;
using Tests.Weapons;
using Tests.Weapons.Launcher;

namespace Tests.BodyBehaviour.Arm
{
    internal class LauncherReload_Obsolete : IArmWeaponBehaviour
    {
        ILauncher _launcher;
        ILauncherDefinitions _definitions;
        internal ITimeline timeline;
        public WeaponType Type => WeaponType.Launcher;

        public IWeapon Weapon
        {
            get => _launcher;
            set
            {
                if (value is ILauncher launcher)
                {
                    _launcher = launcher;
                    _definitions = _launcher.Definitions;
                    if (timeline.IsRunning)
                        throw new TimelineException("The timeline still running, you can't change the timeline right now.");
                    timeline = NewTimeline(_definitions.ReloadDurationTime);
                }
            }
        }
        ITimeline NewTimeline(float duration)
        {
            var t = new Timeline(duration);
            return t;
        }
        public IInput Input { set => throw new NotImplementedException(); }

        public bool Continuing => timeline.IsRunning;

        public bool Begin()
        {
            if (timeline.IsRunning)
                return false;
            return true;
        }

        public bool End()
        {
            if (!timeline.IsRunning)
                return false;
            return true;
        }

    }
}
