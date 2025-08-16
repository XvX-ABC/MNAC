using Assets.Scripts.Utilities.Timeline.Event;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using Assets.Scripts.Utilities.Timeline.Event.Range;
using System;
using UnityEngine;

namespace Assets.Scripts.Utilities.Timeline
{
    public class Timeline : ITimeline
    {

        internal IEventsExecutor[] executors;
        internal float time;
        internal float length;
        internal bool isLoop;
        internal bool isRunning;
        bool _startActionExecuted;

        internal Action<TimelineContext> startAction;
        internal Action<float> updateAction;
        internal Action<TimelineContext> endAction;
        public float Time
        {
            get => time;
        }
        public float NormalizedTime
        {
            get => length == 0 ? 1 : time / length;
        }
        public bool IsRunning
        {
            get => isRunning;
        }
        public float Length
        {
            get => length;
        }
        public Action<TimelineContext> StartAction { get => startAction; set => startAction = value; }
        public Action<float> UpdateAction { get => updateAction; set => updateAction = value; }
        public Action<TimelineContext> EndAction { get => endAction; set => endAction = value; }
        internal Timeline(IEventsExecutor[] executors, float length, bool isLoop, bool isRunning)
        {
            if (executors == null || executors.Length == 0)
                throw new ArgumentException("executor");
            if (length < 0)
                throw new ArgumentException("length");

            this.length = length;
            this.isLoop = isLoop;
            this.isRunning = isRunning;

            this.executors = executors;
        }
        internal Timeline(Span<ITimelineEvent> events, IEventsExecutor[] executors, float length, bool isLoop, bool isRunning) : this(executors, length, isLoop, isRunning)
        {
            if (events == null || events.Length == 0)
                throw new ArgumentException("events");
            foreach (var evt in executors)
                evt.AddEvents(events);
        }
        public Timeline(float duration, bool isLoop, IEventsExecutor[] eventsExecutors, params ITimelineEvent[] events) : this(events, eventsExecutors, duration, isLoop, false) { }
        public Timeline(float duration, bool isLoop, params ITimelineEvent[] events) : this(events, new IEventsExecutor[] { new PointEventsExecutor(), new RangeEventsExecutor() }, duration, isLoop, false)
        {

        }
        public Timeline(float duration, bool isLoop) : this(new IEventsExecutor[] { new PointEventsExecutor(), new RangeEventsExecutor() }, duration, isLoop, false) { }
        public Timeline(float duration) : this(duration, false) { }
        protected Timeline() : this(0) { }
        public virtual void Restart()
        {
            Reset();
            isRunning = true;
        }
        public void Continue()
        {
            isRunning = true;
        }
        public void Pause()
        {
            isRunning = false;
        }
        public void EarlyEnd()
        {
            if (isLoop)
                throw new NotSupportedException("The loop timeline was not supported early end.");
            time = length;
        }
        public void End()
        {
            isRunning = false;
            Reset();
        }
        public void OnUpdate(float deltaTime)
        {
            if (!isRunning)
                return;
            var context = new TimelineContext() { DeltaTime = deltaTime, Time = time, NormalizedTime = length == 0 ? 1 : time / length, Duration = length };

            if (!_startActionExecuted)
            {
                startAction?.Invoke(context);
                _startActionExecuted = true;
            }

            updateAction?.Invoke(time / length);
            foreach (var executor in executors)
                executor.Execute(context);
            if (time >= length)
            {
                endAction?.Invoke(context);
                Reset();
            }
            else
                time += deltaTime;
        }
        protected virtual void ResetExecutors()
        {
            foreach (var executor in executors)
                executor.Reset();
        }
        protected virtual void Reset()
        {
            if (isLoop && time >= length)
                time -= length;
            else
                time = 0;
            isRunning = isLoop;
            _startActionExecuted = false;
            ResetExecutors();
        }

        public bool AddEvent(ITimelineEvent evt)
        {
            if (evt == null)
                return false;
            foreach (var executor in executors)
                if (executor.AddEvent(evt))
                    return true;
            return false;
        }
        public bool RemoveEvent(ITimelineEvent evt)
        {
            if (evt == null)
                return false;
            foreach (var e in executors)
                if (e.RemoveEvent(evt))
                    return true;
            return false;
        }
        public virtual bool UpdateLength(float newLength)
        {
            if (isRunning)
            {
                Debug.LogWarning("This timeline can't update length. because it's running right now.");
                return false;
            }
            if (newLength < 0)
                Debug.LogWarning(new ArgumentException(nameof(newLength)));
            this.length = newLength;
            return true;
        }
        public override string ToString()
        {
            return $"IsRunning: {isRunning}";
        }
    }
}
