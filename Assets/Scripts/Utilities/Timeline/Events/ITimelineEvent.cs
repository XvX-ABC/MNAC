using System;
using Utilities.Timeline;

namespace Utilities.Timeline.Events
{
    public interface ITimelineEvent
    {
        public Func<TimelineContext, bool> Trigger { get; }
        public void Execute(TimelineContext context);
        public void Reset();
    }
}
