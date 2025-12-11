using System;
using Tests.Characters.MountPoints;
using Tests.Weapons;

namespace Tests.Behaviours.Arms.Weapons
{

    public interface IArmedWeaponArmDefinitions
    {
        WeaponDescription[] Origins { get; }
        SwitchingDefinitions Switching { get; }
        MountPointsDefinitions MountPoints { get; }
    }
}
