using UnityEngine;

namespace MNAC.Interaction
{
    public class Target : ITarget_Obsolete
    {
        internal Vector3 pos;
        public Target()
        {
            pos = ITarget_Obsolete.InvalidPosition;
        }
        public Vector3 Position { get => pos; }
    }
}
