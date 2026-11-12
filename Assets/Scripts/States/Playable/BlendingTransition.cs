using System;
using MNAC.Utilities.Timeline;
using UnityEngine;

namespace MNAC.States
{
    /// <summary>
    /// 混合过渡：在 <see cref="PlayableTransition{T}"/> 基础上支持
    /// <paramref name="offset"/>（目标动画从中间开始播，开头段被"吃掉"）与
    /// <paramref name="fixedExitTime"/>（源状态播到指定归一化时刻强制触发）。
    /// 过渡开始时缩短目标 Timeline，目标退出时恢复原长度。
    /// </summary>
    public class BlendingTransition<T> : PlayableTransition<T>, IInterruptiblePlayableTransition<T>
    {
        public const float FIXED_EXIT_TIME_INVALID_VALUE = -1f;

        protected readonly float offset;
        protected readonly float fixedExitTime;
        readonly IWithCallbackPlayableState<T> destinationState;
        IWithCallbackPlayableState<T> shortenedState;
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
            // 过渡被重放/中断重配时，先把上一次的缩短恢复，避免把已缩短长度当成原始长度。
            if (shortenedTimeline != null)
                ResetDestinationState();

            // destinationState 可能是状态机：Timeline 跟随 current 变化，
            // 真正拥有该 Timeline 的是当前子状态，ExitAction 也必须订阅到它身上。
            shortenedState = ResolveTimelineOwner();
            shortenedTimeline = shortenedState.Timeline;
            if (shortenedTimeline == null)
            {
                shortenedState = null;
                return;
            }
            oldLength = shortenedTimeline.Length;
            var newLength = Mathf.Max(0f, oldLength - (offset + ctx.Duration));
            // runningCheck:false —— 修复旧库可能因目标 Timeline 正在运行而静默失效的问题
            shortenedTimeline.UpdateLength(newLength, runningCheck: false);
            shortenedState.ExitAction += ResetDestinationState;
        }

        /// <summary>解析真正持有 Timeline 的状态：包装层取子状态，状态机取 current，普通状态取自身。</summary>
        protected virtual IWithCallbackPlayableState<T> ResolveTimelineOwner()
        {
            if (destinationState is IPlayableStateTimelineOwner<T> owner && owner.TimelineOwner != null)
                return owner.TimelineOwner;
            if (destinationState is StateMachine<T> machine &&
                machine.CurrentState is IWithCallbackPlayableState<T> currentState)
                return currentState;
            return destinationState;
        }

        /// <summary>仅当有活动缩短时恢复，供子类在切换状态前调用。</summary>
        protected void ResetIfActive()
        {
            if (shortenedTimeline != null)
                ResetDestinationState();
        }

        /// <summary>转移被中断时恢复目标 Timeline，避免缩短长度一直残留。</summary>
        public void OnInterrupted() => ResetIfActive();

        protected virtual void ResetDestinationState()
        {
            if (shortenedTimeline != null)
                shortenedTimeline.UpdateLength(oldLength, runningCheck: false);
            if (shortenedState != null)
                shortenedState.ExitAction -= ResetDestinationState;
            shortenedTimeline = null;
            shortenedState = null;
        }
    }
}
