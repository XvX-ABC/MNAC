using System;
using UnityEngine;
namespace Tests.States
{
    [Serializable]

    public class BlendingTransitionOptions : IBlendingTransitionOptions
    {
        [SerializeField]
        float _duration;
        [SerializeField]
        float _offset;
        [SerializeField]
        bool _enableFixedExit;
        [SerializeField]
        float _fixedExitTime;
        [SerializeField]
        InterruptionSource _interruptionSource = BlendingTransition<object>.INTERRUPTION_SOURCE_DEFAULT;

        public float Duration { get => _duration; set => _duration = value; }
        public float Offset { get => _offset; set => _offset = value; }
        public float FixedExitTime { get => _enableFixedExit ? _fixedExitTime : BlendingTransition<object>.FIXED_EXIT_TIME_INVALID_VALUE; set => _fixedExitTime = value; }
        public InterruptionSource InterruptionSource { get => _interruptionSource; set => _interruptionSource = value; }
    }

}