using System;
using UnityEngine;

namespace Locomotion
{
    public class LocomotionDefineBase : MonoBehaviour, ILocomotionDefine
    {
        [Serializable]
        class BaseDefinition : IBaseDefinition
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
        class JumpDefinitionBase : IJumpDefinition
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
        class QuickBoostDefinitionBase : IQuickBoostDefinition
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
        BaseDefinition _base;
        [SerializeField]
        JumpDefinitionBase _jump;
        [SerializeField]
        QuickBoostDefinitionBase _quickBoost;
        public IBaseDefinition Base => _base;

        public IJumpDefinition Jump => _jump;
        public IQuickBoostDefinition QuickBoost => _quickBoost;
    }
}
