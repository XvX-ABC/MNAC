using System;
using UnityEngine;

namespace MNAC.TPhysics.Environment
{
    [Serializable]
    public class GroundDetectionDefinitions : IGroundDetectionDefinitions
    {
        [SerializeField]
        float _maxSlope;
        [SerializeField]
        LayerMask _groundMask;

        public float MaxSlope { get => _maxSlope; }
        public LayerMask GroundMask { get => _groundMask; }
    }
}
