using System;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Sword
{
    [Serializable]
    public class SlashDefinitions : ISlashDefinitions
    {
        [SerializeField]
        float _duration;
        public SlashDefinitions(float duration)
        {
            _duration = Mathf.Max(0, duration);
        }
        public float Duration => _duration;
    }
}
