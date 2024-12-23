using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Utilities.Timeline
{
    public interface ITimelineEvent
    {
        public Func<TimelineContext, bool> Trigger { get; }
        public void Execute(TimelineContext context);
        public void Reset();
    }

    public enum EventType
    {
        Point,
        Range,
    }
    public interface IEventsExecutor
    {
        public bool Initialize(Span<ITimelineEvent> events);
        public void Execute(TimelineContext context);
        public void Reset();

    }
    public struct TimelineContext
    {
        public float Time;
        public float Duration;
        public float Proportion;
        public float DeltaTime;
    }
    public abstract class PointEvent : IPointEvent
    {
        protected Func<TimelineContext, bool> triggeredFunc;
        protected PointEvent(float triggeredProportion)
        {
            triggeredFunc = context =>
            {
                return context.Proportion >= TriggeredProportion;
            };
            _triggeredProportion = triggeredProportion;
        }
        protected float _triggeredProportion;


        public float TriggeredProportion { get => _triggeredProportion; }
        public virtual Func<TimelineContext, bool> Trigger { get => triggeredFunc; }

        public abstract void Execute(TimelineContext context);
        public virtual void Reset() { }
    }
    public abstract class RangeEvent : IRangeEvent
    {
        protected Func<TimelineContext, bool> triggeredFunc;

        protected RangeEvent(float durationProportion, float triggeredProportion)
        {
            triggeredFunc = context =>
            {
                var proportion = context.Proportion;
                var startProportion = TriggeredProportion;
                var endProportion = startProportion + DurationProportion;
                return proportion >= startProportion && proportion < endProportion;
            };
            this.durationProportion = durationProportion;
            this.triggeredProportion = triggeredProportion;
        }

        protected float durationProportion;
        protected float triggeredProportion;
        public Func<TimelineContext, bool> Trigger => triggeredFunc;

        public float DurationProportion { get => durationProportion; }
        public float TriggeredProportion { get => triggeredProportion; }

        public abstract void Execute(TimelineContext context);
        public virtual void Reset() { }
    }
    public interface IPointEvent : ITimelineEvent
    {
        public class Comparer : IComparer<IPointEvent>
        {
            public int Compare(IPointEvent x, IPointEvent y)
            {
                var v0 = x.TriggeredProportion;
                var v1 = y.TriggeredProportion;
                if (v0 < v1)
                    return -1;
                else if (v0 > v1)
                    return 1;
                return 0;
            }
        }
        public float TriggeredProportion { get; }
    }
    public interface IRangeEvent : ITimelineEvent
    {
        public float TriggeredProportion { get; }
        public float DurationProportion { get; }

    }
    public class EventsSequentialExecutor : IEventsExecutor
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
            //_events = events.ToArray();
            //Array.Sort(_events, new IPointEvent.Comparer<T>());
            //Reset();
            return true;
        }

        public void Execute(TimelineContext context)
        {
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
            _nextEventIndex = 0;
            foreach (var evt in _events)
                evt.Reset();
        }
    }
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
        public void Execute(TimelineContext context)
        {
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
            foreach (var evt in _events)
                evt.Reset();
        }
    }
    //public class RangeEventsExecutor : ITimelineEventExecutor
    //{
    //    class EventWrapper
    //    {
    //        public ITimelineEvent Event;
    //        public bool Triggered;
    //    }
    //    EventWrapper[] _events;

    //    public bool Initialize(Span<ITimelineEvent> events, IEventsSorter sorter = null)
    //    {

    //        return true;
    //    }

    //    public void Reset()
    //    {
    //        throw new NotImplementedException();
    //    }
    //    public void Execute(TimelineContext context)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    public class Timeline
    {
        IEventsExecutor[] _executors;
        float _time;
        readonly float _duration;
        readonly bool _isLoop;
        bool _isRunning;
        public float Time
        {
            get => _time;
        }
        public bool IsRunning
        {
            get => _isRunning;
        }
        //internal static IEventsExecutor<T> GetDefaultEventsExecutor()
        //{
        //    var type = typeof(T);
        //    var interfaces = typeof(T).GetInterfaces();
        //    if (interfaces.Contains(typeof(IPointEvent)))
        //        return new EventsSequentialExecutor<T>();
        //    else if (interfaces.Contains(typeof(IRangeEvent)))
        //        return (IEventsExecutor<T>)new RangeEventsExecutor();
        //    return null;
        //}
        internal Timeline(Span<ITimelineEvent> events, IEventsExecutor[] executors, float duration, bool isLoop, bool isRunning)
        {
            if (events == null || events.Length == 0)
                throw new ArgumentException("events");
            if (executors == null || executors.Length == 0)
                throw new ArgumentException("executor");
            if (duration < 0)
                throw new ArgumentException("duration");
            _duration = duration;
            _isLoop = isLoop;
            _isRunning = isRunning;

            var list = new List<IEventsExecutor>();
            foreach (var executor in executors)
            {
                if (executor.Initialize(events))
                    list.Add(executor);
            }
            _executors = list.ToArray();
        }
        public Timeline(Span<ITimelineEvent> events, float duration, bool isLoop) : this(events, new IEventsExecutor[] { new EventsSequentialExecutor(), new RangeEventsExecutor() }, duration, isLoop, false)
        {
        }
        public Timeline(float duration, bool isLoop, IEventsExecutor[] eventsExecutors, params ITimelineEvent[] events) : this(events, eventsExecutors, duration, isLoop, false) { }
        public Timeline(float duration, bool isLoop, params ITimelineEvent[] events) : this(events, duration, isLoop)
        {

        }
        public void Start()
        {
            Reset();
            _isRunning = true;
        }
        public void Continue()
        {
            _isRunning = true;
        }
        public void Stop()
        {
            _isRunning = false;
        }
        public void OnUpdate(float deltaTime)
        {
            if (!_isRunning)
                return;
            var context = new TimelineContext() { DeltaTime = deltaTime, Time = _time, Proportion = _time / _duration, Duration = _duration };
            foreach (var executor in _executors)
                executor.Execute(context);
            if (_time >= _duration)
                Reset();
            else
                _time += deltaTime;
        }
        void ResetExecutors()
        {
            foreach (var executor in _executors)
                executor.Reset();
        }
        void Reset()
        {
            if (_time >= _duration)
                _time -= _duration;
            else
                _time = 0;
            _isRunning = _isLoop;
            ResetExecutors();
        }
        public override string ToString()
        {
            return $"IsRunning: {_isRunning}";
        }
    }
}
