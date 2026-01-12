using System;
using Tests.Weapons;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons
{
    [Serializable]
    public class ArmWeaponDefinitions : IArmedWeaponArmDefinitions
    {
        [SerializeField]
        WeaponDescription[] _origins;
        [SerializeField]
        SwitchingDefinitions _switching;
        [SerializeField]
        MountPointsDefinitions _mountPoints;
        public WeaponDescription[] Origins { get => _origins; }
        public SwitchingDefinitions Switching => _switching;

        public MountPointsDefinitions MountPoints { get => _mountPoints; }
    }
}
