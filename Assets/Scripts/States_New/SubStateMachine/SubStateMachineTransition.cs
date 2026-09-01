using System;
using MNAC.Utilities.Timeline.Events.Point;

namespace MNAC.StatesNew
{
    /// <summary>
    /// 进入子状态机的混合过渡：过渡开始时立即把子机切到指定目标状态
    /// （destinationState 需已注册到 subStateMachine）。
    /// </summary>
    public class SubStateMachineTransition<T> : BlendingTransition<T>
    {
        public SubStateMachineTransition(
            IWithCallbackPlayableState<T> sourceState,
            WithCallbackPlayableStateMachine<T> subStateMachine,
            IWithCallbackPlayableState<T> destinationState,
            Func<bool> trigger = null,
            Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent = null,
            float duration = 0,
            float offset = 0,
            float fixedExitTime = FIXED_EXIT_TIME_INVALID_VALUE,
            InterruptionMode interruptionMode = INTERRUPTION_MODE_DEFAULT)
            : base(sourceState, subStateMachine, trigger, durationEvent, duration, offset, fixedExitTime, interruptionMode)
        {
            timeline.AddPointEvent(0, _ => subStateMachine.ChangeStateTo(destinationState));
        }
    }
}
