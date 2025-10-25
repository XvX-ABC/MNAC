using System;
using UnityEngine;

namespace Tests.Environment
{
    public interface IGroundDetector 
    {
        public IGround CollidedGround { get; }
        public float Distance { get; }
        public float GroundHeight { get; }

    }
}
