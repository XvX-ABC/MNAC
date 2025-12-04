using System;
using Tests.Characters.MountPoints;
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
        string _launcherMountPointName;
        [SerializeField]
        string _swordMountPointName;
        [SerializeField]
        MountPointLocation _launcherMountPointPlace;
        [SerializeField]
        MountPointLocation _swordMountPointPlace;
        [SerializeField]
        float _switchingDurationTime;
        [SerializeField]
        float _switchingMountedProportion;
        [SerializeField]
        float _switchingToBehavioursDurationTime;
        public WeaponDescription[] Origins { get => _origins; }
        public string LauncherMountPointName { get => _launcherMountPointName; }
        public float SwitchingDurationTime { get => _switchingDurationTime; }
        public float SwitchingMountedProportion { get => _switchingMountedProportion; }
        public float SwitchingToBehavioursDurationTime { get => _switchingToBehavioursDurationTime; }

        public string SwordMountPointName => _swordMountPointName;

        public MountPointLocation LauncherMountPointPlace => _launcherMountPointPlace;

        public MountPointLocation SwordMountPointPlace => _swordMountPointPlace;
    }
}
