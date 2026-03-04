
using UnityEngine;

namespace MNAC.Behaviours.Input
{
    public interface IBaseInput
    {
        public Vector3 MousePosition { get; }
        public Vector3 MousePositionDelta { get; }
        public Vector3 HorizontalVector { get; }
    }
}
