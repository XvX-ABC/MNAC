using System;
using UnityEngine;

namespace Tests.Interaction
{
    public interface ICircleRangeTargetsCatcherDefinitions
    {
        [Obsolete]
        public string CatchingObjsTag { get; }
        public ushort FilterAmountOneFrame { get; }
        public LayerMask TargetsMask { get; }
        public float CatchingViewPortRadius { get; }
    }
}
