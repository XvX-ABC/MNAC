using UnityEngine;

namespace Tests.Interaction
{
    public interface ITarget_Obsolete
    {
        internal static Vector3 InvalidPosition = Vector3.positiveInfinity;
        public Vector3 Position { get; }
    }
}
