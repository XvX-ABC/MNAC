using System;
using System.Collections.Generic;
using Tests.Assets;
using UnityEngine;

namespace Tests.Weapons.Launcher
{
    public interface ILauncherDefinitionsEditor
    {
#if UNITY_EDITOR
        public GameObject AmmoOrigin { get; set; }
        public Vector3 MagazinePosition { get; set; }
        public Vector3 MuzzlePosition { get; set; }
        public ushort AmmoSpareQuantity { get; set; }
        public ushort AmmoInMagazineQuantity { get; set; }
        public float ReloadDuration { get; set; }
        public float LaunchDurationTime { get; set; }
        public Vector2 LaunchDelayRange { get; set; }
        public AssetDefinitions OriginAssetDefinitions { get; set; }
        public AssetDefinitions NumericalAssetDefinitions { get; set; }
        public void Save();
        public void Load();
#endif
    }
    public interface ILauncherDefinitions
    {
        public GameObject AmmoOrigin { get; }
        public Vector3 MagazinePosition { get; }
        public Vector3 MuzzlePosition { get; }
        public ushort AmmoTotalQuantity { get; }
        public ushort AmmoSpareQuantity { get; }
        public ushort AmmoInMagazineQuantity { get; }
        public float ReloadDuration { get; }
        public float LaunchDurationTime { get; }
        public Vector2 LaunchDelayRange { get; }
    }
}