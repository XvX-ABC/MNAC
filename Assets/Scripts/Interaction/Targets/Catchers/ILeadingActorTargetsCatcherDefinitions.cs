using System;
using UnityEngine;

namespace MNAC.Interaction
{
    [Obsolete]
    public interface ILeadingActorTargetsCatcherDefinitions
    {
        public LayerMask TerrainMask { get; }
        public LayerMask TargetsMask { get; }
        public Vector4 CatchingRange { get; }
        public bool AllowCatchTerrain { get; }
    }
}
