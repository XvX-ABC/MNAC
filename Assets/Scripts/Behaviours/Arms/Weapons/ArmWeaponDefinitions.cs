using System;
using MNAC.Weapons;
using UnityEngine;

namespace MNAC.Behaviours.Arms.Weapons
{
    [Serializable]
    public class ArmWeaponDefinitions : IArmedArmDefinitions
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
