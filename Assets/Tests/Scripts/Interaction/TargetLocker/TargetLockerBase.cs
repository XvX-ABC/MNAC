using System;

namespace Tests.Interaction
{
    public abstract class TargetLockerBase<T> : ITargetLocker<T> where T : class, ILockTarget
    {
        public abstract bool Enabled { get; set; }
        public abstract T MainLockTarget { get; set; }
        public abstract Action<T, T> MainTargetChangedAction { get; set; }
        public abstract ObstacleDetector ObstacleDetector { get; set; }
        public abstract float TargetChangeDuration { get; set; }
        public abstract bool ObjsCatchEnable { get; set; }
        public abstract bool CursorEnable { get; set; }
        public abstract void OnFixedUpdate();
        public abstract void OnLateUpdate();
    }
}
