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
        [SerializeField]
        float _dragTransitionalDuration;
        [SerializeField]
        Vector2 _dragTransitionalRange;
        public float MaxSpeed => _maxSpeed;
        public float AcceleratedSpeed => _acceleratedSpeed;

        public float DragTransitionalDuration => _dragTransitionalDuration;

        public Vector2 DragTransitionalRange => _dragTransitionalRange;
    }
}
