
using UnityEngine;

namespace MNAC.TPhysics.Environment
{
    public class EnvironmentDefinitions : MonoBehaviour, IEnvironmentDefinitions
    {
        [SerializeField]
        Vector3 _worldUp=World.DefaultUp;
        [SerializeField]
        GroundDetectionDefinitions _groundDetection;
        public Vector3 WorldUpVector => _worldUp;

        public IGroundDetectionDefinitions GroundDetection => _groundDetection;
    }
}
