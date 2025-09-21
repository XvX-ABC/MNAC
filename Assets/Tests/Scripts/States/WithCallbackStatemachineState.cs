using System;

namespace Tests.States
{
    public class WithCallbackStatemachineState<T> : WithCallbackPlayableState<T>
    {
        protected WithCallbackPlayableStatemachine<T> statemachine;
        public WithCallbackStatemachineState(WithCallbackPlayableStatemachine<T> statemachine, string name, float duration = 0, bool enabled = true) : base(name, duration, enabled)
        {
            this.statemachine = statemachine ?? throw new ArgumentNullException(nameof(statemachine));
        }

        public override void OnEnter()
        {
            base.OnEnter();
            statemachine.OnEnter();
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            statemachine.OnUpdate();
        }
        public override void OnExit()
        {
            base.OnExit();
            statemachine.OnExit();
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            statemachine.FromPreviousStateTransitionBegin(currentTransition);
        }
        public override void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.FromPreviousStateTransitionEnd(currentTransition);
            statemachine.FromPreviousStateTransitionEnd(currentTransition);
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            statemachine.FromPreviousStateTransitionRunning(currentTransition);
        }
        public override void ToNextStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.ToNextStateTransitionBegin(currentTransition);
            statemachine.ToNextStateTransitionBegin(currentTransition);
        }
        public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.ToNextStateTransitionEnd(currentTransition);
            statemachine.ToNextStateTransitionEnd(currentTransition);
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            statemachine.ToNextStateTransitionRunning(currentTransition);
        }
    }
}
