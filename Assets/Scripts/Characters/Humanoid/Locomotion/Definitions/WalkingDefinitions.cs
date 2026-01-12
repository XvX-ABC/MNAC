using System;
using UnityEngine;

namespace Tests.Characters.Humanoid.Locomotion
{
    [Serializable]
    public class WalkingDefinitions : IWalkingDefinitions
    {
        [SerializeField]
        float _maxSpeed;
        [SerializeField]
        float _acceleratedSpeed;
        public float MaxSpeed => _maxSpeed;
        public float AcceleratedSpeed => _acceleratedSpeed;

    }
}
