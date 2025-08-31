
using UnityEngine;

namespace Tests.TPhysics.Environment
{
    public interface IEnvironmentDefinitions
    {
        Vector3 WorldUpVector { get; }
        IGroundDetectionDefinitions GroundDetection { get; }
    }
}
