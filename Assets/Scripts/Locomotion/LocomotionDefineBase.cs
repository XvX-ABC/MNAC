using System;
using UnityEngine;

namespace Locomotion
{
    public class LocomotionDefineBase : MonoBehaviour, ILocomotionDefine
    {
        [Serializable]
        class BaseDefines : IBaseDefines
        {
            [SerializeField]
            float _speed;
            [SerializeField]
            float _accelerationSpeed;
            [SerializeField]
            float _ascendingSpeed;
            [SerializeField]
            float _drag;
            public float Speed => _speed;

            public float AccelerationSpeed => _accelerationSpeed;

            public float AscendingSpeed => _ascendingSpeed;
            public float Drag => _drag;
        }
        [Serializable]
        class JumpDefinesBase : IJumpDefines
        {
            [SerializeField]
            float _height;
            [SerializeField]
            float _preparationDuration;
            [SerializeField]
            float _recoverDuration;
            public float Height => _height;

            public float PreparationDuration => _preparationDuration;
            public float LandingDuration => _recoverDuration;

        }
        [Serializable]
        class QuickBoostDefinesBase : IQuickBoostDefines
        {
            [SerializeField]
            float _duration;
            [SerializeField]
            float _interval;
            [SerializeField]
            float _velocity;
            public float Duration => _duration;

            public float Velocity => _velocity;
            public float Interval => _interval;
        }
        [SerializeField]
        BaseDefines _base;
        [SerializeField]
        JumpDefinesBase _jump;
        [SerializeField]
        QuickBoostDefinesBase _quickBoost;
        public IBaseDefines Base => _base;

        public IJumpDefines Jump => _jump;
        public IQuickBoostDefines QuickBoost => _quickBoost;
    }
}
