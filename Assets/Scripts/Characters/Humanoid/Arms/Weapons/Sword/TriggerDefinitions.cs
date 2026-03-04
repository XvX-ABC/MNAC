using System;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Arms.Weapons.Sword
{
    [Serializable]
    public struct TriggerDefinitions
    {
        [SerializeField]
        public LayerMask ExcludeLayerMask;
        [SerializeField]
        public LayerMask IncludeLayerMask;
    }
}
