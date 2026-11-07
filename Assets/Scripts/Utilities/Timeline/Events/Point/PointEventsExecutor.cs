using System;
using System.Collections.Generic;
using System.Linq;
using MNAC.Utilities.Timeline;
using MNAC.Utilities.Timeline.Events;

namespace MNAC.Utilities.Timeline.Events.Point
{
    public class PointEventsExecutor : IEventsExecutor
    {
        IPointEvent[] _events;
        int _nextEventIndex;
        float _lastNormalizedTime;
        bool _hasExecuted;

        internal int nextEventIndex { get => _nextEventIndex; }

        public bool Initialize(Span<ITimelineEvent> events)
        {
            if (events.Length == 0)
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
            RebuildCursor();
            return true;
        }

        int FindIndexByTriggerProportion(float triggerProportion)
        {
            if (_events == null)
                return -1;
            for (var i = 0; i < _events.Length; i++)
            {
                if (_events[i].TriggeredProportion > triggerProportion)
                    return i;
            }
            return -1;
        }

        public bool AddEvent(ITimelineEvent evt)
        {
            if (evt == null || evt is not IPointEvent pevt)
                return false;
            if (_events == null)
            {
                _events = new IPointEvent[] { pevt };
            }
            else
            {
                var index = FindIndexByTriggerProportion(pevt.TriggeredProportion);
                Array.Resize(ref _events, _events.Length + 1);
                if (index == -1)
                {
                    _events[^1] = pevt;
                }
                else
                {
                    Array.Copy(_events, index, _events, index + 1, _events.Length - 1 - index);
                    _events[index] = pevt;
                }
            }
            RebuildCursor();
            return true;
        }

        public ushort AddEvents(Span<ITimelineEvent> events)
        {
            if (events.Length == 0)
                return 0;
            var list = new List<IPointEvent>();
            foreach (var evt in events)
            {
                if (evt is IPointEvent pevt)
                    list.Add(pevt);
            }
            if (list.Count == 0)
                return 0;
            _events = (_events ?? Array.Empty<IPointEvent>())
                .Concat(list)
                .OrderBy(evt => evt.TriggeredProportion)
                .ToArray();
            RebuildCursor();
            return (ushort)list.Count;
        }

        public void RemoveAll()
        {
            _events = null;
            _nextEventIndex = 0;
            _hasExecuted = false;
            _lastNormalizedTime = 0f;
        }

        public bool RemoveEvent(ITimelineEvent evt)
        {
            if (_events == null || evt is not IPointEvent)
                return false;
            var index = Array.IndexOf(_events, evt);
            if (index == -1)
                return false;
            if (index != _events.Length - 1)
                Array.Copy(_events, index + 1, _events, index, _events.Length - index - 1);
            Array.Resize(ref _events, _events.Length - 1);
            if (_events.Length == 0)
                _events = null;
            RebuildCursor();
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
            _lastNormalizedTime = context.NormalizedTime;
            _hasExecuted = true;
            if (_events == null)
                return;
            while (_nextEventIndex < _events.Length)
            {
                var currentEvt = _events[_nextEventIndex];
                if (!currentEvt.Trigger(context))
                    return;
                currentEvt.Execute(context);
                _nextEventIndex++;
            }
        }

        /// 重建游标：已执行过则跳过本次运行中已越过的时刻，否则从头开始。
        void RebuildCursor()
        {
            if (_events == null || !_hasExecuted)
            {
                _nextEventIndex = 0;
                return;
            }
            var index = 0;
            while (index < _events.Length && _events[index].TriggeredProportion <= _lastNormalizedTime)
                index++;
            _nextEventIndex = index;
        }

        public void Reset()
        {
            _nextEventIndex = 0;
            _hasExecuted = false;
            _lastNormalizedTime = 0f;
            if (_events == null)
                return;
            foreach (var evt in _events)
                evt.Reset();
        }
    }
}
