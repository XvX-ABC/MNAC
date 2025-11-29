using Tests.Characters.MountPoints;
﻿using Tests.Weapons;

namespace Tests.Behaviours.Arms.Weapons
{

    public interface IArmedWeaponArmDefinitions
    {
        public WeaponDescription[] Origins { get; }
        public MountPointPlace LauncherMountPointPlace { get; }
        public MountPointPlace SwordMountPointPlace { get; }
        public string LauncherMountPointName { get; }
        public string SwordMountPointName { get; }
        public float SwitchingDurationTime { get; }
        public float SwitchingMountedProportion { get; }
        public float SwitchingToBehavioursDurationTime { get; }
    }
}
