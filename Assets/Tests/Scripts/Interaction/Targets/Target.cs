using UnityEngine;

namespace Tests.Interaction
{
    public class Target : ITarget
    {
        internal Vector3 pos;
        public Target()
        {
            pos = ITarget.InvalidPosition;
        }
        public Vector3 Position { get => pos; }
    }
}
