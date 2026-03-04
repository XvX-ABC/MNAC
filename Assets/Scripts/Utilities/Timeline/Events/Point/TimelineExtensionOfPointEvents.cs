using System;

namespace MNAC.Utilities.Timeline.Events.Point
{
    public static class TimelineExtensionOfPointEvents
    {
        class PointEventWrapper : PointEvent
        {
            internal readonly Action<TimelineContext> action;
            public PointEventWrapper(Action<TimelineContext> func, float triggeredProportion) : base(triggeredProportion)
            {
                action = func ?? throw new ArgumentNullException(nameof(func));
            }

            public override void Execute(TimelineContext context)
            {
                action(context);
            }
        }

        public static ITimelineEvent AddPointEvent(this ITimeline timeline, float triggerProportion, Action<TimelineContext> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (triggerProportion < 0 || triggerProportion > 1)
                throw new ArgumentOutOfRangeException($"The trigger proportion must be in range of 0 and 1.");
            //if (timeline.IsRunning)
            //    throw new InvalidOperationException($"Can't to add the event, because the timeline is running now.");
            var evt = new PointEventWrapper(action, triggerProportion);
            if (!timeline.AddEvent(evt))
                throw new NotSupportedEventTypeException($"Doesn't exist a events executor is support the 'IPointEvent' event type in the timeline.");
            return evt;
        }
        public static bool RemovePointEvent(this ITimeline timeline, ITimelineEvent evt)
        {
            if (evt == null)
                throw new ArgumentNullException(nameof(evt));
            //if (timeline.IsRunning)
            //    throw new InvalidOperationException($"Can't to remove the event, because the timeline is running now.");
            return timeline.RemoveEvent(evt);
        }
    }
}
