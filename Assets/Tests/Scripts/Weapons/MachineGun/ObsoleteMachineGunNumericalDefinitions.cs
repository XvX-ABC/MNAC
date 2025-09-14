using System;
using UnityEngine;

namespace Tests.Weapons.MachineGuns
{
    [Serializable]
    internal class ObsoleteMachineGunNumericalDefinitions
    {
        public Vector3 MagazinePosition;
        public Vector3 MuzzlePosition;
        public ushort AmmoInMagazineQuantity;
        public float ReloadDuration;
        public float RPS;
    }
}
