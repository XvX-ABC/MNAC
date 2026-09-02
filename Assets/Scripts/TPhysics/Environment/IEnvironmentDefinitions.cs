
using UnityEngine;

namespace MNAC.TPhysics.Environment
{
    public interface IEnvironmentDefinitions
    {
        Vector3 WorldUpVector { get; }
        IGroundDetectionDefinitions GroundDetection { get; }
    }
}
