using System;
using MNAC.Utilities.Timeline;

namespace MNAC.States
{
    /// <summary>
    /// 带回调状态机状态：包装 <see cref="WithCallbackPlayableStateMachine{T}"/>，
    /// 生命周期与过渡回调全部转发。进入时确定性进入子机初始态。
    /// </summary>
    public class WithCallbackStateMachineState<T> : WithCallbackPlayableState<T>, IPlayableStateTimelineOwner<T>
    {
        protected readonly WithCallbackPlayableStateMachine<T> stateMachine;

        public WithCallbackStateMachineState(WithCallbackPlayableStateMachine<T> stateMachine, string name, float duration = 0, bool enabled = true)
            : base(name, duration, enabled)
        {
            this.stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        }

        /// <summary>外层使用子机当前状态的 Timeline。</summary>
        public override ITimeline Timeline => stateMachine.Timeline;

        public IWithCallbackPlayableState<T> TimelineOwner => stateMachine.CurrentState as IWithCallbackPlayableState<T>;

        public override void OnEnter()
        {
            base.OnEnter();
            EnsureMachineEntered();
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

        public override string ToString() => stateMachine.ToString();

        void EnsureMachineEntered()
        {
            if (stateMachine.CurrentState == null)
                stateMachine.OnEnter();
        }
    }
}
