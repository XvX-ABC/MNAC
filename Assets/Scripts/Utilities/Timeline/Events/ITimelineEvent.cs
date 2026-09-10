using System;
using MNAC.Utilities.Timeline;

namespace MNAC.Utilities.Timeline.Events
{
    public interface ITimelineEvent
    {
        public Func<TimelineContext, bool> Trigger { get; }
        public void Execute(TimelineContext context);
        public void Reset();
    }
}
