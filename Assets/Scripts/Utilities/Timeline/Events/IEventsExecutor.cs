using System;
using MNAC.Utilities.Timeline;

namespace MNAC.Utilities.Timeline.Events
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
        void RemoveAll();
    }
}
