using Tests.TPhysics;
using UnityEngine;

namespace Tests.Interaction
{
    public interface ITarget
    {
        internal static Vector3 InvalidPosition = TPhysicsHelper.InvalidPosition;
        public Vector3 Position { get; }
    }
}
