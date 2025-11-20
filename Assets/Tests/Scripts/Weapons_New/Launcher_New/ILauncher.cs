using System;
using Tests.Utilities.Timeline;
using UnityEngine;

namespace Tests.Weapons_New.Launcher
{
    internal interface ILauncher
    {
        Transform MuzzleTrans { get; }
        ILauncherDefinitions Definitions { get; }
        ITimeline DelayLaunchTimeline { get; }
        Func<bool> FireTrigger { get; set; }
        Action<ILauncher> LaunchedCallback { get; set; }
        ITimeline LaunchingIntervalTimeline { get; }
        ushort MagazineAmmoAmount { get; }
        string Name { get; }
        Action<ILauncher> ReloadCallback { get; set; }
        ITimeline ReloadTimeline { get; }
        Func<bool> ReloadTrigger { get; set; }
        ushort ReserveAmmoAmount { get; }
        WeaponType Type { get; }

        void FillReserve(int amount);
    }
}