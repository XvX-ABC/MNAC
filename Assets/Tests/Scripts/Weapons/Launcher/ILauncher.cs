using System;
using Tests.Utilities;
using Utilities.Timeline;

namespace Tests.Weapons.Launcher
{
    public interface ILauncher : IWeapon
    {
        public enum ActionsEnum
        {
            None = 0,
            StartLaunch = 1,
            EndLaunch = 2,
            StartReload = 4,
            EndReload = 8,
            Supply = 16,
            All = 255,
        }
        public Action<ILauncher> InitializationAction { get; set; }

        public ITimeline DelayLaunchTimeline { get; }
        public ITimeline LaunchDurationTimeline { get; }
        public ITimeline ReloadTimeline { get; }
        public ILauncherDefinitions Definitions { get; }
        public ushort SpareCount { get; }
        public ushort MagazineCount { get; }
        internal ILauncherActionsLock actionsLock { get; }
        public int Fill(int num);
        public bool StartReload();
        public bool EndReload();
        public bool StartLaunch();
        public bool EndLaunch();
    }
}