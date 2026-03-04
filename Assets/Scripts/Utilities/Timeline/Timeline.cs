using System;
using MNAC.Utilities.Timeline.Events;
using MNAC.Utilities.Timeline.Events.Point;
using MNAC.Utilities.Timeline.Events.Range;
using UnityEngine;
using UTime = UnityEngine.Time;

namespace MNAC.Utilities.Timeline
{
    //public class Timeline_V2 : Timeline_V1
    //{
    //    public Timeline(float duration) : base(duration)
    //    {
    //    }

    //    public Timeline(float duration, bool isLoop) : base(duration, isLoop)
    //    {
    //    }

    //    public override void OnUpdate(float deltaTime)
    //    {
    //        if (!isRunning)
    //            return;
    //        var context = new TimelineContext() { DeltaTime = deltaTime, Time = time, NormalizedTime = length == 0 ? 1 : time / length, Duration = length };

    //        if (!startActionExecuted)
    //        {
    //            startAction?.Invoke(context);
    //            startActionExecuted = true;
    //        }

    //        updateAction?.Invoke(time / length);
    //        foreach (var executor in executors)
    //            executor.Execute(context);
    //        if (time >= length)
    //        {
    //            if (!isLoop)
    //            {
    //                endAction?.Invoke(context);
    //                //Reset();
    //                Pause();
    //            }
    //            else
    //            {
    //                Reset();
    //            }
    //        }
    //        else
    //            time += deltaTime;
    //    }
    //}
    //public class Timeline_V1 : Timeline
    //{
    //    public Timeline(float duration) : base(duration) { }

    //    public Timeline(float duration, bool isLoop) : base(duration, isLoop)
    //    {
    //    }

    //    protected Timeline()
    //    {
    //    }

    //    public override void OnUpdate(float deltaTime)
    //    {
    //        if (!isRunning)
    //            return;
    //        var context = new TimelineContext() { DeltaTime = deltaTime, Time = time, NormalizedTime = length == 0 ? 1 : time / length, Duration = length };

    //        if (!startActionExecuted)
    //        {
    //            startAction?.Invoke(context);
    //            startActionExecuted = true;
    //        }

    //        updateAction?.Invoke(time / length);
    //        foreach (var executor in executors)
    //            executor.Execute(context);
    //        // TOOD: 改善计时接近预定时长时，提前结束
    //        if (time >= length)
    //        {
    //            endAction?.Invoke(context);
    //            //Reset();
    //            Pause();
    //        }
    //        else
    //            time += deltaTime;
    //    }
    //}
    public class Timeline : ITimeline
    {

        internal IEventsExecutor[] executors;
        internal float time;
        internal float length;
        internal bool isLoop;
        internal bool isRunning;
        internal bool startActionExecuted;

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
        public void Start()
        {
            isRunning = true;
        }
        public void Pause()
        {
            isRunning = false;
        }
        public void EndEarly()
        {
            if (isLoop)
                throw new NotSupportedException("The loop timeline was not supported early end.");
            time = length;
            OnUpdate(UTime.deltaTime);
        }
        public void End()
        {
            isRunning = false;
            var ctx = new TimelineContext() { DeltaTime = UTime.deltaTime, Time = time, NormalizedTime = length == 0 ? 1 : time / length, Duration = length };
            endAction?.Invoke(ctx);
        }
        public virtual void OnUpdate(float deltaTime)
        {
            if (!isRunning)
                return;
            var context = new TimelineContext() { DeltaTime = deltaTime, Time = time, NormalizedTime = length == 0 ? 1 : time / length, Duration = length };

            if (!startActionExecuted)
            {
                startAction?.Invoke(context);
                startActionExecuted = true;
            }

            updateAction?.Invoke(time / length);
            foreach (var executor in executors)
                executor.Execute(context);
            if (time >= length)
            {
                if (!isLoop)
                {
                    endAction?.Invoke(context);
                    Pause();
                }
                else
                {
                    Reset();
                }
            }
            else
                time += deltaTime;
        }
        protected virtual void ResetExecutors()
        {
            foreach (var executor in executors)
                executor.Reset();
        }
        public virtual void Reset()
        {
            if (isLoop && time >= length)
                time -= length;
            else
                time = 0;
            isRunning = isLoop & isRunning;
            startActionExecuted = false;
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
        public void RemoveAll()
        {
            foreach (var e in executors)
                e.RemoveAll();
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
        public virtual bool UpdateLength(float newLength, bool runningCheck = true)
        {
            if (runningCheck && isRunning)
            {
                Debug.LogWarning("This timeline can't update length. because it's running right now.");
                return false;
            }
            if (newLength < 0)
                Debug.LogWarning(new ArgumentException(nameof(newLength)));
            length = newLength;
            return true;
        }
        public bool SetTime(float time, bool runningCheck = true)
        {
            if (runningCheck && isRunning)
            {
                Debug.LogWarning("This timeline can't set time. because it's running right now.");
                return false;
            }
            time = Mathf.Clamp(time, 0, length);
            this.time = time;
            return true;
        }
        public bool SetNormalizedTime(float normalizedTime, bool runningCheck = true)
        {
            if (runningCheck && isRunning)
            {
                Debug.LogWarning("This timeline can't set normalized time. because it's running right now.");
                return false;
            }
            normalizedTime = Mathf.Clamp01(normalizedTime);
            time = normalizedTime * length;
            return true;
        }
        public override string ToString()
        {
            return $"IsRunning: {isRunning}";
        }
    }
}
