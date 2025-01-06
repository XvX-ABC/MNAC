using UnityEngine;

namespace Tests.Weapons
{
    public interface ILauncherDefines
    {
        public GameObject ProjectileOrigin { get; }
        public Vector3 BorePosition { get; }
        public Vector3 MuzzlePosition { get; }
        public float FiringRate { get; }
        public ushort ProjectilesTotalNum { get; }
        public ushort ProjectilesTotalNumInMagazine { get; }
        //public ushort BulletsTotalNumInBore { get; }
        public float ReloadDuration { get; }
    }
}