using System;
using UnityEngine;

namespace Tests.Locomotion.Animation
{
    [Serializable]
    class JumpLocomotionAnimationDefinitions : IJumpLocomotionAnimationDefinitions
    {

        [SerializeField]
        float _ascendingClipLength;
        [SerializeField]
        string _ascendingMultiplierName;
        [SerializeField]
        float _landingClipLength;
        [SerializeField]
        string _landingMultiplierName;
        [SerializeField]
        string _landingValueParamName;
        [SerializeField]
        string _descendingClipName;
        [SerializeField]
        float _descendingClipLength;
        [SerializeField]
        string _enterParamName;

        public float AscendingClipLength { get => _ascendingClipLength; }
        public string AscendingMultiplierName { get => _ascendingMultiplierName; }
        public float LandingClipLength { get => _landingClipLength; }
        public string LandingMultiplierName { get => _landingMultiplierName; }
        //public string LandingValueParamName { get => _landingValueParamName; }
        public string DescendingClipName { get => _descendingClipName; }
        public float DescendingClipLength { get => _descendingClipLength; }
        public string EnterParamName { get => _enterParamName; }
    }
}