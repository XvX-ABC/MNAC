using System;
using UnityEngine;

namespace Tests.Locomotion
{
    public interface IGroundDetector : ICollisionDetector
    {
        public IGround CollidedGround { get; }
        public float Distance { get; }
        public float GroundHeight { get; }
    }
}
