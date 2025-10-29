using System;
using UnityEngine;

namespace Tests.Locomotion_Obsolete.Animation
{
    [Serializable]
    public class BoostingLocomotionAnimatorDefinitions : IBoostingLocomotionAnimatorDefinitions
    {
        [SerializeField]
        string _enterParamName;
        [SerializeField]
        string _preparationMultiplierParamName;
        [SerializeField]
        string _durationMultiplierParamName;
        [SerializeField]
        float _boostingClipLength;
        [SerializeField]
        string _toJumpParamName;
        [SerializeField]
        [Range(0, 1)]
        float _preparatoryProportion;
        public string EnterParamName => _enterParamName;

        public string PreparationMultiplierParamName => _preparationMultiplierParamName;
        public string DurationMultiplierParamName => _durationMultiplierParamName;

        public float BoostingClipLength => _boostingClipLength;
        public string ToJumpParamName => _toJumpParamName;

        public float PreparatoryProportion => _preparatoryProportion;
    }
}