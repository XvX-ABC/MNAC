using UnityEngine;

namespace Tests.TPhysics.Environment
{
    public class GroundDetectionDefinitions_MonoComponent : MonoBehaviour, IGroundDetectionDefinitions
    {
        [SerializeField]
        float _maxSlope;
        [SerializeField]
        LayerMask _groundMask;
        [SerializeField]
        Vector3 _worldUpVector;

        public float MaxSlope { get => _maxSlope; }
        public LayerMask GroundMask { get => _groundMask; }
        public Vector3 WorldUpVector { get => _worldUpVector; set => _worldUpVector = value; }
    }
}
