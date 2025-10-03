using UnityEngine;

namespace Tests.Interaction
{
    public interface ICircleRangeTargetsCatcherDefinitions
    {
        public string CatchingObjsTag { get; }
        public ushort FilterCountOneFrame { get; }
        public LayerMask TargetsMask { get; }
        public float CatchingViewPortRadius { get; }
    }
}
