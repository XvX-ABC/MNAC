using System;
using UnityEngine;

namespace MNAC.Behaviours.Arms.Weapons.Sword.Animations
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
