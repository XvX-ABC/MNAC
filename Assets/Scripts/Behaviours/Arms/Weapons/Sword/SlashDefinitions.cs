using System;
using UnityEngine;

namespace MNAC.Behaviours.Arms.Weapons.Sword
{
    [Serializable]
    public class SlashDefinitions : ISlashDefinitions
    {
        [SerializeField]
        float _duration;
        [SerializeField]
        float _recoveryDuration;
        public SlashDefinitions(float duration, float recoveryDuration)
        {
            _duration = Mathf.Max(0, duration);
            _recoveryDuration = Mathf.Max(0, recoveryDuration);
        }
        public float Duration => _duration;

        public float RecoveryDuration { get => _recoveryDuration; }
    }
}
