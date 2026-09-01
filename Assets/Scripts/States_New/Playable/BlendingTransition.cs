using System;
using MNAC.Utilities.Timeline;
using UnityEngine;

namespace MNAC.StatesNew
{
    /// <summary>
    /// 混合过渡：在 <see cref="PlayableTransition{T}"/> 基础上支持
    /// <paramref name="offset"/>（目标动画从中间开始播，开头段被"吃掉"）与
    /// <paramref name="fixedExitTime"/>（源状态播到指定归一化时刻强制触发）。
    /// 过渡开始时缩短目标 Timeline，目标退出时恢复原长度。
    /// </summary>
    public class BlendingTransition<T> : PlayableTransition<T>
    {
        public const float FIXED_EXIT_TIME_INVALID_VALUE = -1f;

        protected readonly float offset;
        protected readonly float fixedExitTime;
        readonly IWithCallbackPlayableState<T> destinationState;
        ITimeline shortenedTimeline;
        float oldLength;

        public BlendingTransition(
            IWithCallbackPlayableState<T> source,
            IWithCallbackPlayableState<T> destination,
            Func<bool> trigger = null,
            Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent = null,
            float duration = 0,
            float offset = 0,
            float fixedExitTime = FIXED_EXIT_TIME_INVALID_VALUE,
            InterruptionMode interruptionMode = INTERRUPTION_MODE_DEFAULT)
            : base(source, destination, trigger, durationEvent, duration, interruptionMode)
        {
            this.offset = Mathf.Max(0f, offset);
            this.fixedExitTime = fixedExitTime == FIXED_EXIT_TIME_INVALID_VALUE
                ? fixedExitTime
                : Mathf.Clamp01(fixedExitTime);
            destinationState = destination;

            if (this.fixedExitTime != FIXED_EXIT_TIME_INVALID_VALUE)
                AddTrigger(() => PlayableSource.Timeline.NormalizedTime >= this.fixedExitTime);

            Timeline.StartAction += Begin;
        }

        public BlendingTransition(
            IWithCallbackPlayableState<T> source,
            IWithCallbackPlayableState<T> destination,
            Func<bool> trigger,
            Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent,
            BlendingTransitionOptions options)
            : this(source, destination, trigger, durationEvent, options.Duration, options.Offset, options.FixedExitTime, options.InterruptionMode)
        {
        }

        protected virtual void Begin(TimelineContext ctx)
        {
            // 锁定同一个 Timeline 实例：子状态机等场景中 destinationState.Timeline 会随 current 变化，
            // 若在 Reset 时再动态取属性会恢复错对象。
            shortenedTimeline = destinationState.Timeline;
            if (shortenedTimeline == null)
                return;
            oldLength = shortenedTimeline.Length;
            var newLength = Mathf.Max(0f, oldLength - (offset + ctx.Duration));
            // runningCheck:false —— 修复旧库可能因目标 Timeline 正在运行而静默失效的问题
            shortenedTimeline.UpdateLength(newLength, runningCheck: false);
            destinationState.ExitAction += ResetDestinationState;
        }

        protected virtual void ResetDestinationState()
        {
            if (shortenedTimeline != null)
                shortenedTimeline.UpdateLength(oldLength, runningCheck: false);
            shortenedTimeline = null;
            destinationState.ExitAction -= ResetDestinationState;
        }
    }
}
