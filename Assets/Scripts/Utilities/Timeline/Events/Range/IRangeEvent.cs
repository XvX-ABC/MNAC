using Utilities.Timeline.Events;

namespace Utilities.Timeline.Events.Range
{
    public interface IRangeEvent : ITimelineEvent
    {
        public float TriggeredProportion { get; }
        public float DurationProportion { get; }

    }
}
