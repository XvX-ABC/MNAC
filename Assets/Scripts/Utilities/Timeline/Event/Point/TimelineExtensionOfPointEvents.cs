using System;
using System.Linq;

namespace Assets.Scripts.Utilities.Timeline.Event.Point
{
    internal static class TimelineExtensionOfPointEvents
    {
        class PointEventWrapper : PointEvent
        {
            Action<TimelineContext> _action;
            public PointEventWrapper(Action<TimelineContext> func, float triggeredProportion) : base(triggeredProportion)
            {
                _action = func ?? throw new ArgumentNullException(nameof(func));
            }

            public override void Execute(TimelineContext context)
            {
                _action(context);
            }
        }

        public static void AddPointEvent(this Timeline timeline, float triggerProportion, Action<TimelineContext> action)
        {
            if (timeline.isRunning)
                throw new InvalidOperationException($"Can't to add the event, because the timeline is running now.";
            var executor = timeline.executors.FirstOrDefault(evt => evt is PointEventsExecutor);
            if (executor == null)
                throw new Exception($"Doesn't exist a events executor in the timeline support 'IPointEvent' type.");
            executor.AddEvent(new PointEventWrapper(action, triggerProportion));
        }
    }
}
