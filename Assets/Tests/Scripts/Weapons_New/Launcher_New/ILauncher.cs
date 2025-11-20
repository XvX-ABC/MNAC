using System;
using Tests.Utilities.Timeline;
using UnityEngine;

namespace Tests.Weapons_New.Launcher
{
    public interface ILauncher : IWeapon
    {
        Transform MuzzleTrans { get; }
        ILauncherDefinitions Definitions { get; }
        ITimeline DelayLaunchTimeline { get; }
        Func<bool> FireTrigger { get; set; }
        Action<ILauncher> LaunchedCallback { get; set; }
        ITimeline LaunchingIntervalTimeline { get; }
        ushort MagazineAmmoAmount { get; }
        Action<ILauncher> ReloadCallback { get; set; }
        ITimeline ReloadTimeline { get; }
        Func<bool> ReloadTrigger { get; set; }
        ushort ReserveAmmoAmount { get; }

        void FillReserve(int amount);
    }
}