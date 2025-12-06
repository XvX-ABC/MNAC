
using UnityEngine;

namespace Tests.Characters.Humanoid.Locomotion
{
    public interface IMovementDefinitions
    {
        public float MaxSpeed { get; }
        public float AcceleratedSpeed { get; }
        public float DragTransitionalDuration { get; }
        public Vector2 DragTransitionalRange { get; }
    }
}
