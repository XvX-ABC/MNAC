using Assets.Scripts.Utilities.Timeline.Event;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using Assets.Scripts.Utilities.Timeline.Event.Range;
using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Utilities.Timeline
{
    public class Timeline : ITimeline
    {

        internal IEventsExecutor[] executors;
        internal float time;
        internal float duration;
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
        public bool IsRunning
        {
            get => isRunning;
        }
        public float Length
        {
            get => duration;
        }
        public Action<TimelineContext> StartAction { get => startAction; set => startAction = value; }
        public Action<float> UpdateAction { get => updateAction; set => updateAction = value; }
        public Action<TimelineContext> EndAction { get => endAction; set => endAction = value; }
        internal Timeline(IEventsExecutor[] executors, float duration, bool isLoop, bool isRunning)
        {
            if (executors == null || executors.Length == 0)
                throw new ArgumentException("executor");
            if (duration < 0)
                throw new ArgumentException("duration");

            this.duration = duration;
            this.isLoop = isLoop;
            this.isRunning = isRunning;

            this.executors = executors;
        }
        internal Timeline(Span<ITimelineEvent> events, IEventsExecutor[] executors, float duration, bool isLoop, bool isRunning) : this(executors, duration, isLoop, isRunning)
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
        public virtual void Start()
        {
            Reset();
            isRunning = true;
        }
        public void Continue()
        {
            isRunning = true;
        }
        public void Stop()
        {
            isRunning = false;
        }
        public void EarlyEnd()
        {
            if (isLoop)
                throw new NotSupportedException("The loop timeline was not supported early end.");
            time = duration;
        }
        public void OnUpdate(float deltaTime)
        {
            if (!isRunning)
                return;
            var context = new TimelineContext() { DeltaTime = deltaTime, Time = time, Proportion = time / duration, Duration = duration };

            if (!_startActionExecuted)
            {
                startAction?.Invoke(context);
                _startActionExecuted = true;
            }

            updateAction?.Invoke(time / duration);
            foreach (var executor in executors)
                executor.Execute(context);
            if (time >= duration)
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
            if (isLoop && time >= duration)
                time -= duration;
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
        public bool UpdateLength(float newLength)
        {
            if (isRunning)
            {
                Debug.LogWarning("The timeline can't update length now. because it's running");
                return false;
            }
            if (newLength < 0)
                Debug.LogWarning(new ArgumentException(nameof(newLength)));
            this.duration = newLength;
            return true;
        }
        public override string ToString()
        {
            return $"IsRunning: {isRunning}";
        }
    }
}
