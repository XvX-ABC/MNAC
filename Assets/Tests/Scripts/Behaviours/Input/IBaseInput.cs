
using UnityEngine;

namespace Tests.Behaviours.Input
{
    public interface IBaseInput
    {
        public Vector3 MousePosition { get; }
        public Vector3 MousePositionDelta { get; }
        public Vector3 HorizontalVector { get; }
    }
}
