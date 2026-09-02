using UnityEngine;

namespace MNAC.TPhysics.Environment
{
    public class EnvironmentCore
    {
        World _world;
        GroundDetector _groundDetector;
        public EnvironmentCore(Vector3 worldUpVector, IGroundDetectionDefinitions groundDefinitions)
        {
            _world = new(worldUpVector, Physics.gravity);
            _groundDetector = new(groundDefinitions, _world);
        }
        public EnvironmentCore(IEnvironmentDefinitions definitions) : this(definitions.WorldUpVector, definitions.GroundDetection)
        {

        }

        public World World { get => _world; }
        public GroundDetector GroundDetector { get => _groundDetector; }
    }
}
