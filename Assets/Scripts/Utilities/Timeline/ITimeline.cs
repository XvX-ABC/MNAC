using System;
using MNAC.Utilities.Timeline.Events;

namespace MNAC.Utilities.Timeline
{
    public interface IReadonlyTimeline
    {
        bool IsRunning { get; }
        float Time { get; }
        float NormalizedTime { get => 0f; }
        float Length { get; }
        string ToString();
    }
    public interface ITimeline : IReadonlyTimeline
    {
        Action<TimelineContext> StartAction { get; set; }
        Action<float> UpdateAction { get; set; }
        Action<TimelineContext> EndAction { get; set; }
        void Start();
        void OnUpdate(float deltaTime);
        void Restart();
        void Pause();
        void EndEarly();
        bool AddEvent(ITimelineEvent evt);
        bool RemoveEvent(ITimelineEvent evt);
        bool UpdateLength(float newLength, bool runningCheck = true);
        bool SetNormalizedTime(float normalizedTime, bool runningCheck = true);
        bool SetTime(float time, bool runningCheck = true);
        void End();
        void Reset();
        void RemoveAll();
    }
}