using Assets.Scripts.Utilities.Timeline.Event.Point;
using System;
using System.Linq;

namespace Assets.Scripts.Utilities.Timeline.Event.Range
{
    internal static class TimelineExtensionOfRangeEvents
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
        public static ITimelineEvent AddRangeEvent(this Timeline timeline, float triggerProportion, float durationProportion, Action<TimelineContext> action)
        {
            if (timeline.isRunning)
                throw new InvalidOperationException($"Can't to add the event, because the timeline is running now.");
            var executor = timeline.executors.FirstOrDefault(evt => evt is RangeEventsExecutor);
            if (executor == null)
                throw new NotSupportedEventTypeException($"Doesn't exist a events executor is support the 'IRangeEvent' event type in the timeline.");
            var evt = new RangeEventWrapper(action, durationProportion, triggerProportion);
            executor.AddEvent(evt);
            return evt;
        }
        public static bool RemoveRangeEvent(this Timeline timeline, ITimelineEvent evt)
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
