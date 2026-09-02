using UnityEngine;

namespace MNAC.Weapons.Launcher
{
    public interface ILauncherDefinitions
    {
        public ushort AmmoReserveAmount { get; }
        public ushort AmmoInMagazineAmount { get; }
        public ushort AmmoTotalAmount { get => (ushort)(AmmoReserveAmount + AmmoInMagazineAmount); }
        public float ReloadDurationTime { get; }
        public float LaunchingIntervalTime { get; }
        public Vector2 LaunchDelayRange { get; }
        bool AllowedAutoReload { get; }
    }
}
