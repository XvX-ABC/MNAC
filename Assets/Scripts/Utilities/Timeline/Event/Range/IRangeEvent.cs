namespace Assets.Scripts.Utilities.Timeline.Event.Range
{
    public interface IRangeEvent : ITimelineEvent
    {
        public float TriggeredProportion { get; }
        public float DurationProportion { get; }

    }
}
