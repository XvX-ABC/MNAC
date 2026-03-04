using UnityEngine;

namespace MNAC.Interaction
{
    public abstract class PlayerTargetLockerBase<T> : TargetLockerBase<T>, IPlayerTargetLocker<T> where T : class, ILockTarget
    {
        public abstract bool ObjsCatchEnable { get; set; }
        public abstract bool CursorEnable { get; set; }
        public abstract float CatchAngle { get; set; }
        public abstract Vector3 CursorPosition { get; set; }
        public abstract Vector3 CursorPositionDelta { get; set; }
        public abstract Vector3 OriginWorldPosition { get; set; }
    }
}
