using System;
using UnityEngine;

namespace Tests.BodyBehaviour.Arm
{
    [Serializable]
    public class ArmWeaponDefinitions : IArmWeaponDefinitions
    {
        [SerializeField]
        ArmWeaponDescription[] _origins;
        [SerializeField]
        string _mountPointName;
        [SerializeField]
        float _switchingDurationTime;
        [SerializeField]
        float _switchingMountedProportion;
        public ArmWeaponDescription[] Origins { get => _origins; }
        public string MountPointName { get => _mountPointName; }
        public float SwitchingDurationTime { get => _switchingDurationTime; }
        public float SwitchingMountedProportion { get => _switchingMountedProportion; }
    }
}
