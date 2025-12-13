using System;
using System.Linq;
using Tests.Utilities.Timeline.Events.Point;

namespace Tests.Utilities.Timeline.Events.Range
{
    public static class TimelineExtensionOfRangeEvents
    {
        class RangeEventWrapper : RangeEvent
        {
            Action<TimelineContext> _action;

            public RangeEventWrapper(Action<TimelineContext> func, float durationProportion, float triggeredProportion) : base(durationProportion, triggeredProportion)
            {
                _action = func ?? throw new ArgumentNullException(nameof(func));
            }

            public override void Execute(TimelineContext context)
            {
                _action(context);
            }
        }
        public static ITimelineEvent AddRangeEvent(this ITimeline timeline, float triggerProportion, float durationProportion, Action<TimelineContext> action)
        {
            if (timeline.IsRunning)
                throw new InvalidOperationException($"Can't to add the event, because the timeline is running now.");
            if (triggerProportion < 0 || triggerProportion > 1)
                throw new ArgumentOutOfRangeException($"The trigger proportion must be in range of 0 and 1.");
            if (timeline.IsRunning)
                throw new InvalidOperationException($"Can't to add the event, because the timeline is running now.");
            var evt = new RangeEventWrapper(action, durationProportion, triggerProportion);
            if (!timeline.AddEvent(evt))
                throw new NotSupportedEventTypeException($"Doesn't exist a events executor is support the 'IPointEvent' event type in the timeline.");
            return evt;
        }
        public static bool RemoveRangeEvent(this ITimeline timeline, ITimelineEvent evt)
        {
            if (evt == null)
                throw new ArgumentNullException(nameof(evt));
            if (timeline.IsRunning)
                throw new InvalidOperationException($"Can't to add the event, because the timeline is running now.");
            //var executor = timeline.executors.FirstOrDefault(executor => executor is PointEventsExecutor);
            //if (executor == null)
            //    throw new NotSupportedEventTypeException($"Doesn't exist a events executor is support the 'IPointEvent' event type in the timeline.");
            return timeline.RemoveEvent(evt);
        }
    }
}
