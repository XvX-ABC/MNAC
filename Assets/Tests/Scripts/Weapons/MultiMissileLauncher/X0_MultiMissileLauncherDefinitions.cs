using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons
{
    [Serializable]
    public class Ammo
    {
        public GameObject Origin;
        public ushort SpareQuantity;
        public ushort InMagazineQuantity;
    }
    [Serializable]
    public class MountPoints
    {
        public Vector3 MagazinePosition;
        public Vector3 MuzzlePosition;
    }
    [Serializable]
    public class Durations
    {
        public Vector2 LaunchDelay;
        public float ReloadDuration
        { get => CoverOpenOrCloseDuration + MagazineFullOrEmptyDuration * 2; }
        public float CoverOpenOrCloseDuration;
        public float MagazineFullOrEmptyDuration;
    }
    [DisallowMultipleComponent]
    public class X0_MultiMissileLauncherDefinitions : MonoBehaviour, IMissileLauncherDefinitions, ILauncherAnimatorActionDefinitions
    {
        [SerializeField]
        Ammo _ammo;
        [SerializeField]
        MountPoints _mountPoints;
        [SerializeField]
        Durations _durations;

        public Vector2 LaunchDelayRange => _durations.LaunchDelay;

        public GameObject AmmoOrigin => _ammo.Origin;

        public Vector3 MagazinePosition => _mountPoints.MagazinePosition;

        public Vector3 MuzzlePosition => _mountPoints.MuzzlePosition;

        public ushort AmmoTotalQuantity => (ushort)(AmmoSpareQuantity + AmmoInMagazineQuantity);

        public ushort AmmoSpareQuantity => _ammo.SpareQuantity;

        public ushort AmmoInMagazineQuantity => _ammo.InMagazineQuantity;

        public float ReloadDuration => _durations.ReloadDuration;

        public float CoverOpenOrCloseDuration => _durations.CoverOpenOrCloseDuration;

        public float MagazineFullOrEmptyDuration => _durations.MagazineFullOrEmptyDuration;

        //public Dictionary<ushort, IABAssetSaver> AssetSavers => throw new NotImplementedException();
    }
}
