using System;
using MNAC.Utilities.Timeline;

namespace MNAC.States
{
    /// <summary>
    /// 子状态机状态：包装一个 <see cref="WithCallbackPlayableStateMachine{T}"/>，
    /// 进入时把子机切到指定目标状态（targetState 需已注册到子机），并透传过渡回调。
    /// </summary>
    public class SubStateMachineState<T> : WithCallbackPlayableState<T>, IPlayableStateTimelineOwner<T>
    {
        readonly WithCallbackPlayableStateMachine<T> stateMachine;
        readonly IWithCallbackPlayableState<T> targetState;

        public SubStateMachineState(
            WithCallbackPlayableStateMachine<T> stateMachine,
            IWithCallbackPlayableState<T> targetState,
            string name,
            bool enabled = true)
            : base(name, 0, enabled)
        {
            this.stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
            this.targetState = targetState ?? throw new ArgumentNullException(nameof(targetState));
            if (!stateMachine.Contains(targetState))
                throw new StateNotRegisteredException(targetState.Name);
            timeline = new Timeline(targetState.Timeline?.Length ?? 0f);
        }

        /// <summary>外层使用子机当前状态的 Timeline，而不是包装层那条独立 Timeline。</summary>
        public override ITimeline Timeline => stateMachine.Timeline;

        public IWithCallbackPlayableState<T> TimelineOwner => stateMachine.CurrentState as IWithCallbackPlayableState<T>;

        public override void OnEnter()
        {
            base.OnEnter();
            EnsureMachineEntered();
            stateMachine.ChangeStateTo(targetState);
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            stateMachine.OnUpdate(deltaTime);
        }

        public override void OnExit()
        {
            base.OnExit();
            stateMachine.OnExit();
        }

        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            EnsureMachineEntered();
            stateMachine.ChangeStateTo(targetState);
            stateMachine.FromPreviousStateTransitionBegin(currentTransition);
        }

        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            stateMachine.FromPreviousStateTransitionRunning(currentTransition);
        }

        public override void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.FromPreviousStateTransitionEnd(currentTransition);
            stateMachine.FromPreviousStateTransitionEnd(currentTransition);
        }

        public override void ToNextStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.ToNextStateTransitionBegin(currentTransition);
            stateMachine.ToNextStateTransitionBegin(currentTransition);
        }

        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            stateMachine.ToNextStateTransitionRunning(currentTransition);
        }

        public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.ToNextStateTransitionEnd(currentTransition);
            stateMachine.ToNextStateTransitionEnd(currentTransition);
        }

        void EnsureMachineEntered()
        {
            if (stateMachine.CurrentState == null)
                stateMachine.OnEnter();
        }
    }
}
