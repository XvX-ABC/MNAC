using System;
using MNAC.Utilities.Timeline;

namespace MNAC.States
{
    /// <summary>
    /// 进入子状态机的混合过渡：过渡开始时先把子机切到目标状态，再按 BlendingTransition
    /// 的规则缩短目标状态 Timeline（offset/duration 才能真正作用到新目标上）。
    /// </summary>
    public class SubStateMachineTransition<T> : BlendingTransition<T>
    {
        readonly WithCallbackPlayableStateMachine<T> subStateMachine;
        readonly IWithCallbackPlayableState<T> destinationState;

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
            this.subStateMachine = subStateMachine ?? throw new ArgumentNullException(nameof(subStateMachine));
            this.destinationState = destinationState ?? throw new ArgumentNullException(nameof(destinationState));
            if (!subStateMachine.Contains(destinationState))
                throw new StateNotRegisteredException(destinationState.Name);
        }

        protected override void Begin(TimelineContext ctx)
        {
            ResetIfActive(); // 先恢复上一次可能缩短的 Timeline
            EnsureSubMachineEntered();
            subStateMachine.ChangeStateTo(destinationState);
            base.Begin(ctx); // 此时 destination 的 Timeline 已是新目标状态
        }

        void EnsureSubMachineEntered()
        {
            if (subStateMachine.CurrentState == null)
                subStateMachine.OnEnter();
        }
    }
}
