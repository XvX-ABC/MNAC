using System;
using UnityEngine;

namespace Tests.Characters.Humanoid.Locomotion
{
    [Serializable]
    public class QuickBoostingDefinitions : BoostingDefinitions, IQuickBoostingDefinitions
    {
        [SerializeField]
        float _duration;
        [SerializeField]
        float _coldDownTime;
        public float Duration => _duration;
        public float ColdDownTime => _coldDownTime;

    }
}
