using System;
using Tests.Weapons;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons
{
    [Serializable]
    public class ArmWeaponDefinitions : IArmWeaponDefinitions
    {
        [SerializeField]
        WeaponDescription[] _origins;
        [SerializeField]
        string _mountPointName;
        [SerializeField]
        float _switchingDurationTime;
        [SerializeField]
        float _switchingMountedProportion;
        [SerializeField]
        float _switchingToBehavioursDurationTime;
        public WeaponDescription[] Origins { get => _origins; }
        public string MountPointName { get => _mountPointName; }
        public float SwitchingDurationTime { get => _switchingDurationTime; }
        public float SwitchingMountedProportion { get => _switchingMountedProportion; }
        public float SwitchingToBehavioursDurationTime { get => _switchingToBehavioursDurationTime; }
    }
}
