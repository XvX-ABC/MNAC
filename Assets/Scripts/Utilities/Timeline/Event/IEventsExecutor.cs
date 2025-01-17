using System;

namespace Assets.Scripts.Utilities.Timeline.Event
{
    public interface IEventsExecutor
    {
        [Obsolete]
        public bool Initialize(Span<ITimelineEvent> events);
        public bool AddEvent(ITimelineEvent @event);
        public ushort AddEvents(Span<ITimelineEvent> events);
        public bool RemoveEvent(ITimelineEvent @event);
        public ushort RemoveEvents(Span<ITimelineEvent> events);
        public void Execute(TimelineContext context);
        public void Reset();

    }
}
