using System;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Sword
{
    [Serializable]
    public class BoostingDefinitions : IBoostingDefinitions
    {
        [SerializeField]
        float _maxSpeed;
        [SerializeField]
        float _acceleratedSpeed;
        [SerializeField]
        float _maxDuration;
        [SerializeField]
        float _cdDuration;
        public BoostingDefinitions(float maxSpeed, float acceleratedSpeed, float maxDuration)
        {
            _maxSpeed = Mathf.Max(0, maxSpeed);
            _acceleratedSpeed = Mathf.Max(0, acceleratedSpeed);
            _maxDuration = Mathf.Max(0, maxDuration);
        }
        public float MaxSpeed { get => _maxSpeed; }

        public float AcceleratedSpeed => _acceleratedSpeed;

        public float MaxDuration => _maxDuration;

        public float ColdDownDuration => _cdDuration;
    }
}
