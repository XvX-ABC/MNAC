using System;

namespace Assets.Scripts.Utilities.Timeline.Event
{
    public interface ITimelineEvent
    {
        public Func<TimelineContext, bool> Trigger { get; }
        public void Execute(TimelineContext context);
        public void Reset();
    }
}
