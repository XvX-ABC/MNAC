using System;
using Tests.Locomotion;
using UnityEngine;

namespace Locomotion
{
    public interface IGroundDetector : IRayCollisionDetector
    {
        public IGround Ground { get; }
        public float CurrentHeight { get; }


        [Obsolete]
        public bool AutoSample { get; set; }
        [Obsolete]
        public bool TouchedGround { get; }
        [Obsolete]
        public Vector3 Normal { get; }
        [Obsolete]
        public Vector3 Point { get; }
        [Obsolete]
        public void Sample();
    }
}
