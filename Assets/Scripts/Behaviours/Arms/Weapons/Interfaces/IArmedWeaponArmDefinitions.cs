using System;
using MNAC.Characters.MountPoints;
using MNAC.Weapons;

namespace MNAC.Behaviours.Arms.Weapons
{

    public interface IArmedArmDefinitions
    {
        WeaponDescription[] Origins { get; }
        SwitchingDefinitions Switching { get; }
        MountPointsDefinitions MountPoints { get; }
    }
}
