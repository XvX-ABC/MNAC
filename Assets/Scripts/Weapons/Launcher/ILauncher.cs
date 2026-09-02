using System;
using MNAC.Interaction;
using MNAC.Utilities.Timeline;
using MNAC.Weapons.Projectiles;
using UnityEngine;

namespace MNAC.Weapons.Launcher
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
        LayerMask LayerMaskToHit { get; set; }
        TeamMask TeamMask { get; set; }
        Action<int, int> MagazineAmountChangeAction { get; set; }
        Action<int, int> ReserveAmountChangeAction { get; set; }

        void FillReserve(int amount);
    }
}