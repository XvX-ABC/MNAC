using System;
using UnityEngine;
namespace Tests.Locomotion.Animation
{
    [Serializable]
    class AirLocomotionAnimationDefinitions : IAirLocomotionAnimationDefinitions
    {
        [SerializeField]
        string _enterParamName;
        [SerializeField]
        string _xParamName;
        [SerializeField]
        string _yParamName;
        [SerializeField]
        string _descentClipName;
        [SerializeField]
        string _nextStateClipName;
        [SerializeField]
        float _v0;

        public string EnterParamName => _enterParamName;

        public string XParamName => _xParamName;

        public string YParamName => _yParamName;

        public string DescentClipName => _descentClipName;

        public string NextStateClipName => _nextStateClipName;

        public float V0 => _v0;
    }
}