using UnityEngine;

namespace MNAC.TPhysics.Environment
{
    public interface IGroundDetectionDefinitions
    {
        LayerMask GroundMask { get; }
        float MaxSlope { get; }
    }
}