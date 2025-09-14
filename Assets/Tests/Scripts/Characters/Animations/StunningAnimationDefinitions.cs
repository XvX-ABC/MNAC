using System;
using UnityEngine;

namespace Tests.Characters.Animations
{
    [Serializable]
    public class StunningAnimationDefinitions : IStunningAnimationDefinitions
    {
        [SerializeField]
        float _clipLength;
        [SerializeField]
        string _multiplier;
        [SerializeField]
        string _trigger;
        public float ClipLength => _clipLength;

        public string Multiplier => _multiplier;

        public string Trigger => _trigger;
    }
}
