using Assets.Scripts.Utilities.Timeline.Event;
using System;

namespace Assets.Scripts.Utilities.Timeline
{
    public interface ITimeline
    {
        bool IsRunning { get; }
        float Time { get; }
        float Length { get; }
        Action<TimelineContext> StartAction { get; set; }
        Action<float> UpdateAction { get; set; }
        Action<TimelineContext> EndAction { get; set; }
        void Continue();
        void OnUpdate(float deltaTime);
        void Start();
        void Stop();
        void EarlyEnd();
        string ToString();
        bool AddEvent(ITimelineEvent evt);
        bool RemoveEvent(ITimelineEvent evt);
        bool UpdateLength(float newLength);
    }
}