using Assets.Scripts.Utilities.Timeline.Event.Range;
using System;
using System.Linq;

namespace Assets.Scripts.Utilities.Timeline.Event.Point
{
    internal static class TimelineExtensionOfPointEvents
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

        public static ITimelineEvent AddPointEvent(this Timeline timeline, float triggerProportion, Action<TimelineContext> action)
        {
            if (timeline.isRunning)
                throw new InvalidOperationException($"Can't to add the event, because the timeline is running now.");
            var executor = timeline.executors.FirstOrDefault(executor => executor is PointEventsExecutor);
            if (executor == null)
                throw new NotSupportedEventTypeException($"Doesn't exist a events executor is support the 'IPointEvent' event type in the timeline.");
            var evt = new PointEventWrapper(action, triggerProportion);
            executor.AddEvent(evt);
            return evt;
        }
        public static bool RemovePointEvent(this Timeline timeline, ITimelineEvent evt)
        {
            if (evt == null)
                throw new ArgumentNullException(nameof(evt));
            if (timeline.isRunning)
                throw new InvalidOperationException($"Can't to add the event, because the timeline is running now.");
            var executor = timeline.executors.FirstOrDefault(executor => executor is PointEventsExecutor);
            if (executor == null)
                throw new NotSupportedEventTypeException($"Doesn't exist a events executor is support the 'IPointEvent' event type in the timeline.");
            return executor.RemoveEvent(evt);
        }
    }
}
