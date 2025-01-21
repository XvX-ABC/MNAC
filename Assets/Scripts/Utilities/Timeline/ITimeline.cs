using Assets.Scripts.Utilities.Timeline.Event;

namespace Assets.Scripts.Utilities.Timeline
{
    public interface ITimeline
    {
        bool IsRunning { get; }
        float Time { get; }

        void Continue();
        void OnUpdate(float deltaTime);
        void Start();
        void Stop();
        string ToString();
        bool AddEvent(ITimelineEvent evt);
        bool RemoveEvent(ITimelineEvent evt);
    }
}