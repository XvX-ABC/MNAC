using System;
using UnityEngine;
namespace Tests.Locomotion.Animation
{
    [Serializable]
    class AirLocomotionAnimationDefines : IAirLocomotionAnimationDefines
    {
        [SerializeField]
        string _enterParamName;
        [SerializeField]
        string _xParamName;
        [SerializeField]
        string _yParamName;
        public string EnterParamName => _enterParamName;

        public string XParamName => _xParamName;

        public string YParamName => _yParamName;
    }
}