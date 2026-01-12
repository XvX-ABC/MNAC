using Tests.Utilities.Timeline.Events;

namespace Tests.Utilities.Timeline.Events.Range
{
    public interface IRangeEvent : ITimelineEvent
    {
        public float TriggeredProportion { get; }
        public float DurationProportion { get; }

    }
}
