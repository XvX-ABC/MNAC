using System;
using UnityEngine;

namespace Tests.Locomotion.Animation
{
    [Serializable]
    public class BoostingLocomotionAnimatorDefinitions : IBoostingLocomotionAnimatorDefinitions
    {
        [SerializeField]
        string _enterParamName;
        [SerializeField]
        string _speedMultiplierParamName;
        [SerializeField]
        float _boostingClipLength;
        [SerializeField]
        string _toJumpParamName;
        public string EnterParamName => _enterParamName;

        public string SpeedMultiplierParamName => _speedMultiplierParamName;

        public float BoostingClipLength => _boostingClipLength;
        public string ToJumpParamName => _toJumpParamName;
    }
}