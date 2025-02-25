using Assets.Tests.Scripts.Weapons.Assets;
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
        public float LaunchRate;
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
        public float LaunchDelay => throw new System.NotImplementedException();

        public Vector2 LaunchDelay_New => _durations.LaunchDelay;

        public GameObject AmmoOrigin => _ammo.Origin;

        public Vector3 MagazinePosition => _mountPoints.MagazinePosition;

        public Vector3 MuzzlePosition => _mountPoints.MuzzlePosition;

        public float LaunchRate => _durations.LaunchRate;

        public ushort AmmoTotalQuantity => (ushort)(AmmoSpareQuantity + AmmoQuantityInMagazine);

        public ushort AmmoSpareQuantity => _ammo.SpareQuantity;

        public ushort AmmoQuantityInMagazine => _ammo.InMagazineQuantity;

        public float ReloadDuration => _durations.ReloadDuration;

        public float CoverOpenOrCloseDuration => _durations.CoverOpenOrCloseDuration;

        public float MagazineFullOrEmptyDuration => _durations.MagazineFullOrEmptyDuration;

        public Dictionary<ushort, IABAssetSaver> AssetSavers => throw new NotImplementedException();
    }
}
