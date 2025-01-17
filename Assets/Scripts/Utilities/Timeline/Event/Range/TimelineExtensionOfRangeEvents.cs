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
        public static void AddRangeEvent(this Timeline timeline, float triggerProportion, float durationProportion, Action<TimelineContext> action)
        {
            if (timeline.isRunning)
                throw new InvalidOperationException($"Can't to add the event, because the timeline is running now.");
            var executor = timeline.executors.FirstOrDefault(evt => evt is RangeEventsExecutor);
            if (executor == null)
                throw new Exception($"Doesn't exist a events executor in the timeline support 'IRangeEvent' type.");
            executor.AddEvent(new RangeEventWrapper(action, durationProportion, triggerProportion));
        }
    }
}
