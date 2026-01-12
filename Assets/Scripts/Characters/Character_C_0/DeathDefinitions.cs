using System;
using UnityEngine;

namespace Tests.Characters.C_0
{
    [Serializable]
    internal class DeathDefinitions : IDeathDefinitions
    {
        [SerializeField]
        float _delayDestroyDuration = 3;

        public float DelayDestroyDuration { get => _delayDestroyDuration; set => _delayDestroyDuration = value; }
    }
}
