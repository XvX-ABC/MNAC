using System;
using MNAC.Utilities.Timeline.Events;
using MNAC.Utilities.Timeline.Events.Point;
using MNAC.Utilities.Timeline.Events.Range;
using UnityEngine;

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
        internal bool endActionExecuted;

        internal Action<TimelineContext> startAction;
        internal Action<float> updateAction;
        internal Action<TimelineContext> endAction;
        public float Time
        {
            get => time;
        }
        public float NormalizedTime
        {
            get => length <= 0f ? 1f : Mathf.Clamp01(time / length);
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
            if (events.Length == 0)
                return;
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
            time = 0f;
            isRunning = true;
        }
        public void Start()
        {
            // End()/EndEarly() 或自然播完后 endActionExecuted 会保持 true；
            // 此时 Start 视为开始新一轮，而不是从半途续跑（否则 End 将永远不再触发）。
            if (endActionExecuted || time >= length)
                Reset();
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
            if (endActionExecuted)
                return; // 已经结束过：不要重复触发 UpdateAction / 事件
            if (time < length)
                time = length;
            isRunning = true;
            OnUpdate(0f);
        }
        public void End()
        {
            if (endActionExecuted)
                return;
            isRunning = false;
            endActionExecuted = true;
            var ctx = new TimelineContext()
            {
                DeltaTime = 0f,
                Time = time,
                NormalizedTime = this.NormalizedTime,
                Duration = length,
            };
            endAction?.Invoke(ctx);
        }
        public virtual void OnUpdate(float deltaTime)
        {
            if (!isRunning)
                return;
            var normalized = NormalizedTime;
            var context = new TimelineContext() { DeltaTime = deltaTime, Time = time, NormalizedTime = normalized, Duration = length };

            if (!startActionExecuted)
            {
                startAction?.Invoke(context);
                startActionExecuted = true;
            }

            updateAction?.Invoke(normalized);
            foreach (var executor in executors)
                executor.Execute(context);
            if (time >= length)
            {
                if (!isLoop)
                {
                    if (!endActionExecuted)
                    {
                        endActionExecuted = true;
                        endAction?.Invoke(context);
                    }
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
                time = length > 0f ? Mathf.Repeat(time, length) : 0f; // 大 delta 时一次取模，避免逐帧追帧
            else
                time = 0;
            isRunning = isLoop & isRunning;
            startActionExecuted = false;
            endActionExecuted = false;
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
            if (newLength < 0f)
            {
                Debug.LogWarning($"Timeline length can't be negative: {newLength}");
                return false;
            }
            length = newLength;
            if (time > length)
                time = length;
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
