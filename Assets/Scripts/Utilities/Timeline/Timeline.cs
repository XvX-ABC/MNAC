using Assets.Scripts.Utilities.Timeline.Event;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using Assets.Scripts.Utilities.Timeline.Event.Range;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Utilities.Timeline
{

    public class Timeline : ITimeline
    {

        internal IEventsExecutor[] executors;
        internal float time;
        internal readonly float duration;
        internal readonly bool isLoop;
        internal bool isRunning;
        public float Time
        {
            get => time;
        }
        public bool IsRunning
        {
            get => isRunning;
        }
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
        public void Start()
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
        public void OnUpdate(float deltaTime)
        {
            if (!isRunning)
                return;
            var context = new TimelineContext() { DeltaTime = deltaTime, Time = time, Proportion = time / duration, Duration = duration };
            foreach (var executor in executors)
                executor.Execute(context);
            if (time >= duration)
                Reset();
            else
                time += deltaTime;
        }
        void ResetExecutors()
        {
            foreach (var executor in executors)
                executor.Reset();
        }
        void Reset()
        {
            if (time >= duration)
                time -= duration;
            else
                time = 0;
            isRunning = isLoop;
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
        public override string ToString()
        {
            return $"IsRunning: {isRunning}";
        }
    }
}
