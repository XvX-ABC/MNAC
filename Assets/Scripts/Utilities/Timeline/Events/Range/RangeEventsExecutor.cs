using System;
using System.Collections.Generic;
using System.Linq;
using MNAC.Utilities.Timeline;
using MNAC.Utilities.Timeline.Events;

namespace MNAC.Utilities.Timeline.Events.Range
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
            bool _hasEntered;
            public Func<TimelineContext, bool> Trigger { get => Event.Trigger; }

            /// <summary>
            /// 本帧是否应执行。除正常逐帧命中区间外，还处理“一帧跨过整个区间”的情况：
            /// 若区间从未被执行过，则补一次，避免短窗口被整帧跳过。
            /// </summary>
            public bool ShouldExecute(float currentProportion)
            {
                if (Triggered)
                    return false;
                var start = Event.TriggeredProportion;
                var end = start + Event.DurationProportion;
                if (currentProportion < start)
                    return false;
                if (currentProportion <= end)
                    return true;
                return !_hasEntered; // 已经进入过则不再补，直接跨过才算漏掉
            }

            public void UpdateTriggeredState(float currentProportion)
            {
                Triggered = currentProportion >= Event.TriggeredProportion + Event.DurationProportion;
            }
            public void Execute(TimelineContext context)
            {
                _hasEntered = true;
                Event.Execute(context);
                UpdateTriggeredState(context.NormalizedTime);
            }

            public void Reset()
            {
                Triggered = false;
                _hasEntered = false;
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
            Array.Sort(_events, new Comparer());
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
            _events = (_events ?? Array.Empty<InternalRangeEvent>())
                .Concat(list)
                .OrderBy(evt => evt.Event.TriggeredProportion)
                .ToArray();
            return (ushort)list.Count;
        }
        public void RemoveAll()
        {
            _events = null;
        }
        public bool RemoveEvent(ITimelineEvent evt)
        {
            if (_events == null || evt == null || evt is not IRangeEvent)
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
            if (index != _events.Length - 1)
                Array.Copy(_events, index + 1, _events, index, _events.Length - index - 1);
            Array.Resize(ref _events, _events.Length - 1);
            if (_events.Length == 0)
                _events = null;
            return true;
        }
        public ushort RemoveEvents(Span<ITimelineEvent> events)
        {
            if (_events == null || events.Length == 0)
                return 0;
            ushort removed = 0;
            foreach (var evt in events)
            {
                if (RemoveEvent(evt))
                    removed++;
            }
            return removed;
        }
        public void Execute(TimelineContext context)
        {
            if (_events == null)
                return;
            var currentProportion = context.NormalizedTime;
            foreach (var evt in _events)
            {
                if (evt.Triggered)
                    continue;
                if (evt.ShouldExecute(currentProportion))
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
