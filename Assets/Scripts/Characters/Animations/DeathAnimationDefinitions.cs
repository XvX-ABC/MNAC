using System;
using MNAC.Characters.Animations;
using UnityEngine;

namespace MNAC.Characters
{
    [Serializable]
    public class DeathAnimationDefinitions : IDeathAnimationDefinitions
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
