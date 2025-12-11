using System;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Sword.Animations
{
    [Serializable]
    public struct ActionDefinitions
    {
        [SerializeField]
        public string SwitchName;
        [SerializeField]
        public string MultiplierName;
        [SerializeField]
        public float ClipLength;
    }
}
