
using UnityEngine;

namespace MNAC.Characters.Humanoid.Locomotion
{
    public interface IMovementDefinitions
    {
        public float MaxSpeed { get; }
        public float AcceleratedSpeed { get; }
    }
}
