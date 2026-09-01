using System;

namespace MNAC.StatesNew
{
    /// <summary>
    /// 可播放状态机状态：包装 <see cref="PlayableStateMachine{T}"/>，
    /// 生命周期与过渡回调全部转发。进入时确定性进入子机初始态。
    /// </summary>
    public class PlayableStateMachineState<T> : WithCallbackPlayableState<T>
    {
        readonly PlayableStateMachine<T> stateMachine;

        public PlayableStateMachineState(PlayableStateMachine<T> stateMachine, string name, float duration = 0, bool enabled = true)
            : base(name, duration, enabled)
        {
            this.stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        }

        public override void OnEnter()
        {
            base.OnEnter();
            stateMachine.OnEnter();
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
    }
}
