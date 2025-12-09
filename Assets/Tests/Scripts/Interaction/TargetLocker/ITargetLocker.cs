using System;

namespace Tests.Interaction
{
    public interface ITargetLocker<T> where T : class, ILockTarget
    {
        bool Enabled { get; set; }
        T MainLockTarget { get; set; }
        Action<T, T> MainTargetChangedAction { get; set; }
        ObstacleDetector ObstacleDetector { get; set; }
        float TargetChangeDuration { get; set; }

        void OnFixedUpdate();
        void OnLateUpdate();
    }
}