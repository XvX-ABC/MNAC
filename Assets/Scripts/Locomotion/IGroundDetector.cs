using System;
using Tests.Locomotion;
using UnityEngine;

namespace Locomotion
{
    public interface IGroundDetector 
    {
        public IGround CollidedGround { get; }
        public float Distance { get; }
        public float GroundHeight { get; }
    }
}
