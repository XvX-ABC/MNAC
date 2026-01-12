using System;

namespace Tests.Interaction
{
    public abstract class TargetLockerBase<T> : ITargetLocker<T> where T : class, ILockTarget
    {
        protected bool enabled;
        public virtual bool Enabled { get => enabled; set => enabled = value; }
        public abstract T MainLockTarget { get; set; }
        public abstract Action<T, T> MainTargetChangedAction { get; set; }
        public abstract ObstacleDetector ObstacleDetector { get; set; }
        public abstract float TargetChangeDuration { get; set; }

        public abstract void OnFixedUpdate();
        public abstract void OnLateUpdate();
    }
}
