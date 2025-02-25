using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Utilities.Timeline.Event.Point
{
    public class PointEventsExecutor : IEventsExecutor
    {
        IPointEvent[] _events;
        byte _nextEventIndex;
        internal byte nextEventIndex { get => _nextEventIndex; }

        public bool Initialize(Span<ITimelineEvent> events)
        {
            if (events == null || events.Length == 0)
                return false;
            var list = new List<IPointEvent>();
            foreach (var evt in events)
            {
                if (evt is IPointEvent pevt)
                    list.Add(pevt);
            }
            if (list.Count == 0)
                return false;
            _events = list.OrderBy(evt => evt.TriggeredProportion).ToArray();
            return true;
        }
        int FindIndexByTriggerProportion(float triggerProportion)
        {
            for (var i = 0; i < _events.Length; i++)
            {
                var t = _events[i].TriggeredProportion;
                if (t > triggerProportion)
                    return i;
            }
            return -1;
        }
        public bool AddEvent(ITimelineEvent evt)
        {
            if (evt == null || evt is not IPointEvent pevt)
                return false;
            if (_events == null)
                _events = new IPointEvent[] { pevt };
            else
            {
                var index = FindIndexByTriggerProportion(pevt.TriggeredProportion);
                Array.Resize(ref _events, _events.Length + 1);
                if (index == -1)
                    _events[^1] = pevt;
                else
                {
                    Array.Copy(_events, index, _events, index + 1, _events.Length - 1 - index);
                    _events[index] = pevt;
                }
            }
            return true;
        }
        public ushort AddEvents(Span<ITimelineEvent> events)
        {
            if (events == null || events.Length == 0)
                return 0;
            var list = new List<IPointEvent>();
            foreach (var evt in events)
            {
                if (evt is IPointEvent pevt)
                    list.Add(pevt);
            }
            if (list.Count == 0)
                return 0;
            _events = list.OrderBy(evt => evt.TriggeredProportion).ToArray();
            return (ushort)list.Count;
        }
        public bool RemoveEvent(ITimelineEvent evt)
        {
            if (evt == null || evt is not IPointEvent pevt)
                return false;
            var index = Array.IndexOf(_events, evt);
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
                var i = Array.IndexOf(_events, evt);
                if (i == -1)
                    continue;
                existenceCount++;
            }
            return existenceCount;
        }
        public void Execute(TimelineContext context)
        {
            if (_events == null)
                return;
            ref var index = ref _nextEventIndex;
            var currentEvt = default(ITimelineEvent);
            do
            {
                if (index >= _events.Length)
                    return;
                currentEvt = _events[index];
                if (currentEvt.Trigger(context))
                {
                    currentEvt.Execute(context);
                    index++;
                }
                else
                    currentEvt = null;
            } while (currentEvt != null);
        }

        public void Reset()
        {
            if (_events == null)
                return;
            _nextEventIndex = 0;
            foreach (var evt in _events)
                evt.Reset();
        }
    }
}
