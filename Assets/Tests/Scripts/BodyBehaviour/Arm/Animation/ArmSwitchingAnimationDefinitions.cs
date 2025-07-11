using System;
using UnityEngine;

namespace Assets.Tests.Scripts.BodyBehaviour.Arm.Animations
{
    [Serializable]
    public class ArmSwitchingAnimationDefinitions : IArmSwitchingAnimationDefinitions
    {
        [SerializeField]
        string _enterName;
        [SerializeField]
        string _multiplierName;
        [SerializeField]
        float _clipLength;

        public string EnterName { get => _enterName; }
        public string MultiplierName { get => _multiplierName; }
        public float ClipLength { get => _clipLength; }
    }
}
