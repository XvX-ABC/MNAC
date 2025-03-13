using UnityEngine;

namespace Locomotion
{
    public interface IRayCollisionDetector
    {
        public LayerMask TargetLayerMask { get; set; }
        public float Length { get; set; }
        public Vector3 RelativeDirection { get; set; }
        public Vector3 OriginOffset { get; set; }
    }
}
