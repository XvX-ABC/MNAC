using System;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Locomotion
{
    [Serializable]
    public class BoostingDefinitions : IBoostingDefinitions
    {
        [SerializeField]
        float _speedPower;
        [SerializeField]
        float _acceleratedSpeedPower;
        public float MaxSpeedPower => _speedPower;
        public float AcceleratedSpeedPower => _acceleratedSpeedPower;
    }
}
