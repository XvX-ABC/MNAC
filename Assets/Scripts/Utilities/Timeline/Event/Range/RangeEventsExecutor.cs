using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Utilities.Timeline.Event.Range
{
    public class RangeEventsExecutor : IEventsExecutor
    {
        class Comparer : IComparer<InternalRangeEvent>
        {
            public int Compare(InternalRangeEvent x, InternalRangeEvent y)
            {
                var v0 = x.Event.TriggeredProportion;
                var v1 = y.Event.TriggeredProportion;
                if (v0 < v1)
                    return -1;
                else if (v0 > v1)
                    return 1;
                return 0;
            }
        }
        class InternalRangeEvent : ITimelineEvent
        {
            public IRangeEvent Event;
            public bool Triggered;
            public Func<TimelineContext, bool> Trigger { get => Event.Trigger; }
            public void UpdateTriggeredState(float currentProportion)
            {
                Triggered = currentProportion >= Event.TriggeredProportion + Event.DurationProportion;
            }
            public void Execute(TimelineContext context)
            {
                Event.Execute(context);
                UpdateTriggeredState(context.Proportion);
            }

            public void Reset()
            {
                Triggered = false;
                Event.Reset();
            }
        }
        InternalRangeEvent[] _events;
        public bool Initialize(Span<ITimelineEvent> events)
        {
            if (events == null || events.Length == 0)
                return false;
            var list = new List<InternalRangeEvent>();
            foreach (var evt in events)
                if (evt is IRangeEvent revt)
                    list.Add(new() { Event = revt });
            if (list.Count == 0)
                return false;
            _events = list.OrderBy(evt => evt.Event.TriggeredProportion).ToArray();
            Reset();
            return true;
        }
        public bool AddEvent(ITimelineEvent evt)
        {
            if (evt == null || evt is not IRangeEvent revt)
                return false;
            if (_events != null)
                Array.Resize(ref _events, _events.Length + 1);
            else
                _events = new InternalRangeEvent[1];
            _events[^1] = new() { Event = revt };
            return true;
        }
        public ushort AddEvents(Span<ITimelineEvent> events)
        {
            if (events == null || events.Length == 0)
                return 0;
            var list = new List<InternalRangeEvent>();
            foreach (var evt in events)
                if (evt is IRangeEvent revt)
                    list.Add(new() { Event = revt });
            if (list.Count == 0)
                return 0;
            _events = list.OrderBy(evt => evt.Event.TriggeredProportion).ToArray();
            return (ushort)list.Count;
        }
        public bool RemoveEvent(ITimelineEvent evt)
        {
            if (evt == null || evt is not IRangeEvent revt)
                return false;
            var index = -1;
            for (var i = 0; i < _events.Length; i++)
            {
                var internalEvent = _events[i];
                if (internalEvent.Event == evt)
                    index = i;
            }
            if (index == -1)
                return false;
            Array.Copy(_events, index + 1, _events, index, _events.Length - index + 1);
            Array.Resize(ref _events, _events.Length - 1);
            return true;
        }
        public ushort RemoveEvents(Span<ITimelineEvent> events)
        {
            if (events == null || events.Length == 0)
                return 0;
            var existenceCount = (ushort)0;
            foreach (var evt in events)
            {
                var index = -1;
                for (var i = 0; i < _events.Length; i++)
                {
                    var internalEvent = _events[i];
                    if (internalEvent.Event == evt)
                        index = i;
                }
                if (index == -1)
                    continue;
                existenceCount++;
            }
            return existenceCount;
        }
        public void Execute(TimelineContext context)
        {
            if (_events == null)
                return;
            var currentProportion = context.Proportion;
            foreach (var evt in _events)
            {
                if (evt.Triggered)
                    continue;
                if (evt.Trigger(context))
                    evt.Execute(context);
            }
        }

        public void Reset()
        {
            if (_events == null)
                return;
            foreach (var evt in _events)
                evt.Reset();
        }
    }
}
