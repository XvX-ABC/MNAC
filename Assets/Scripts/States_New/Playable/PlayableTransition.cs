using System;
using MNAC.Utilities.Timeline;
using MNAC.Utilities.Timeline.Events.Range;

namespace MNAC.StatesNew
{
    /// <summary>
    /// 可播放转移：自建一条 Timeline，把六个过渡回调（源/目标 × Begin/Running/End）
    /// 与可选的逐帧 <paramref name="durationEvent"/> 挂到时间线事件上。
    /// </summary>
    public class PlayableTransition<T> : Transition<T>, IPlayableTransition<T>
    {
        public const InterruptionMode INTERRUPTION_MODE_DEFAULT = InterruptionMode.Next;

        protected readonly ITimeline timeline;
        readonly InterruptionMode interruptionMode;

        public PlayableTransition(
            IPlayableState<T> source,
            IPlayableState<T> destination,
            Func<bool> trigger = null,
            Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent = null,
            float duration = 0,
            InterruptionMode interruptionMode = INTERRUPTION_MODE_DEFAULT)
            : base(source, destination, trigger)
        {
            this.interruptionMode = interruptionMode;
            timeline = new Timeline(duration);
            if (durationEvent != null)
                timeline.AddRangeEvent(0, 1, ctx => durationEvent.Invoke(source, destination, ctx.NormalizedTime));
            HookTransitionEvents();
        }

        public ITimeline Timeline => timeline;
        public InterruptionMode InterruptionMode => interruptionMode;

        protected IPlayableState<T> PlayableSource => Source as IPlayableState<T>;
        protected IPlayableState<T> PlayableDestination => Destination as IPlayableState<T>;

        void HookTransitionEvents()
        {
            timeline.StartAction += _ => PlayableSource?.ToNextStateTransitionBegin(this);
            timeline.AddRangeEvent(0, 1, _ => PlayableSource?.ToNextStateTransitionRunning(this));
            timeline.EndAction += _ => PlayableSource?.ToNextStateTransitionEnd(this);

            timeline.StartAction += _ => PlayableDestination?.FromPreviousStateTransitionBegin(this);
            timeline.AddRangeEvent(0, 1, _ => PlayableDestination?.FromPreviousStateTransitionRunning(this));
            timeline.EndAction += _ => PlayableDestination?.FromPreviousStateTransitionEnd(this);
        }
    }
}
