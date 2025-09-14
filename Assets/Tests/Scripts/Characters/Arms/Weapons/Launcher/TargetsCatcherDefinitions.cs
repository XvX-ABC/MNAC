using System;
using UnityEngine;

namespace Tests.Characters.Arms.Weapons.Launchers
{
    [Serializable]
    public class TargetsCatcherDefinitions : ITargetsCatcherDefinitions
    {
        [SerializeField]
        protected LayerMask terrainMask;
        [SerializeField]
        protected LayerMask targetsMask;
        [SerializeField]
        protected Vector4 catchingRange;
        [SerializeField]
        protected bool allowCatchTerrain;
        public LayerMask TerrainMask => terrainMask;

        public LayerMask TargetsMask => targetsMask;

        public Vector4 CatchingRange => catchingRange;

        public bool AllowCatchTerrain => allowCatchTerrain;
    }
}
