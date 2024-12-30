using System;
using UnityEngine;

namespace Tests.Locomotion.Animation
{
    [Serializable]
    public class HorizontalLocomotionAnimationDefines : IHorizontalLocomotionAnimationDefines
    {
        [SerializeField]
        string _xParamName;
        [SerializeField]
        string _yParamName;
        public string XParamName => _xParamName;

        public string YParamName => _yParamName;
    }
}