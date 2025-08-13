using Assets.Scripts.Utilities.Timeline.Event;
using System;

namespace Assets.Scripts.Utilities.Timeline
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
        void Continue();
        void OnUpdate(float deltaTime);
        void Restart();
        void Pause();
        void EarlyEnd();
        bool AddEvent(ITimelineEvent evt);
        bool RemoveEvent(ITimelineEvent evt);
        bool UpdateLength(float newLength);
        void End();
    }
}