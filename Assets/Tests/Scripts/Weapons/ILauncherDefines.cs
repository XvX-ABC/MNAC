using UnityEngine;

namespace Tests.Weapons
{
    public interface ILauncherDefines
    {
        public GameObject AmmoOrigin { get; }
        public Vector3 MagazinePosition { get; }
        public Vector3 MuzzlePosition { get; }
        public float LaunchRate { get; }
        public ushort AmmoTotalQuantity { get; }
        public ushort AmmoSpareQuantity { get; }
        public ushort AmmoQuantityInMagazine { get; }
        //public ushort BulletsTotalNumInBore { get; }
        public float ReloadDuration { get; }
    }
}