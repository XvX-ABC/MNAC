using System;
using System.Security;
using UnityEngine;

namespace Tests.Locomotion.Animation
{
    [Serializable]
    public class GroundLocomotionAnimatorDefinitions
        : IGroundLocomotionAnimatorDefinitions
    {
        [SerializeField]
        string _xParamName;
        [SerializeField]
        string _yParamName;
        [SerializeField]
        string _stateHoldingParamName;
        public string XParamName => _xParamName;

        public string YParamName => _yParamName;
        public string StateHoldingParamName=>_stateHoldingParamName;
    }
}