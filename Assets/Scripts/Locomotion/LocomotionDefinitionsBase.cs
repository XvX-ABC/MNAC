using System;
using UnityEngine;

namespace Locomotion
{
    public class LocomotionDefinitionsBase : MonoBehaviour, ILocomotionDefinitions
    {
        [Serializable]
        class BaseDefinitions : IBaseDefinitions
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
        class JumpDefinitionsBase : IJumpDefinitions
        {
            [SerializeField]
            float _height;
            [SerializeField]
            float _preparationDuration;
            [Obsolete]
            float _recoverDuration;
            public float Height => _height;

            public float PreparationDuration => _preparationDuration;
            public float LandingDuration => _recoverDuration;

        }
        [Serializable]
        class BoostingDefinitions : IBoostingDefinitions
        {
            [SerializeField]
            float _duration;
            [SerializeField]
            float _interval;
            [SerializeField]
            float _power;
            public float Duration => _duration;

            public float Interval => _interval;
            public float Power => _power;
        }
        [SerializeField]
        BaseDefinitions _base;
        [SerializeField]
        JumpDefinitionsBase _jump;
        [SerializeField]
        BoostingDefinitions _quickBoost;
        public IBaseDefinitions Base => _base;

        public IJumpDefinitions Jump => _jump;
        public IBoostingDefinitions Boosting => _quickBoost;
    }
}
