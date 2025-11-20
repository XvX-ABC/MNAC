using System;
using Tests.Behaviours.Arms.Weapons.Launcher;
using Tests.Utilities.Timeline;

namespace Tests.Weapons.Launcher
{
    public interface ILauncher_Obsolete : IWeapon_Obsolete
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
        [Obsolete]
        public Action<ILauncher_Obsolete> InitializationAction { get; set; }
        public Action<ILauncher_Obsolete> LaunchAction { get; set; }
        public Action<ILauncher_Obsolete> ReloadAction { get; set; }
        public ITimeline DelayLaunchTimeline { get; }
        public ITimeline LaunchDurationTimeline { get; }
        public ITimeline ReloadTimeline { get; }
        public ILauncherDefinitions Definitions { get; }
        public ushort ReservesAmmoCount { get; }
        public ushort MagazineAmmoCount { get; }
        internal ILauncherActionsLock actionsLock { get; }
        public int Fill(int num);
        public bool StartReload();
        public bool EndReload();
        public bool StartLaunch();
        public bool EndLaunch();
    }
}