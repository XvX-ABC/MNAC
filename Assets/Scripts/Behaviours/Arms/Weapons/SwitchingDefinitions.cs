using System;
using UnityEngine;

namespace MNAC.Behaviours.Arms.Weapons
{
    [Serializable]
    public struct SwitchingDefinitions
    {
        [SerializeField]
        public float DurationTime;
        [SerializeField]
        public float MountedProportion;
    }
}
