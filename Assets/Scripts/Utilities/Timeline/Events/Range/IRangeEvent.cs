using MNAC.Utilities.Timeline.Events;

namespace MNAC.Utilities.Timeline.Events.Range
{
    public interface IRangeEvent : ITimelineEvent
    {
        public float TriggeredProportion { get; }
        public float DurationProportion { get; }

    }
}
