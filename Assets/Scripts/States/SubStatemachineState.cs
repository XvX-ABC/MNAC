using Tests.Utilities.Timeline;
using UnityEngine;

namespace Tests.States
{
    public class SubStatemachineState<T> : WithCallbackPlayableState<T>
    {
        WithCallbackPlayableStatemachine<T> _statemachine;
        IWithCallbackPlayableState<T> _state;
        public SubStatemachineState(WithCallbackPlayableStatemachine<T> statemachine, IWithCallbackPlayableState<T> targetState, string name, bool enabled = true) : base(name, 0, enabled)
        {
            _statemachine = statemachine;
            _state = targetState;
            this.timeline = new Timeline(_state.Timeline.Length);
        }
        public override void OnEnter()
        {
            base.OnEnter();
            timeline.Restart();
            _statemachine.OnEnter();
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            timeline.OnUpdate(Time.deltaTime);
            _statemachine.OnUpdate();
        }
        public override void OnExit()
        {
            base.OnExit();
            timeline.End();
            _statemachine.OnExit();
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            _statemachine.ChangeStateTo(_state);
            _statemachine.FromPreviousStateTransitionBegin(currentTransition);
        }
        public override void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.FromPreviousStateTransitionEnd(currentTransition);
            _statemachine.FromPreviousStateTransitionEnd(currentTransition);
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            _statemachine.FromPreviousStateTransitionRunning(currentTransition);
        }
        public override void ToNextStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.ToNextStateTransitionBegin(currentTransition);
            _statemachine.ToNextStateTransitionBegin(currentTransition);
        }
        public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.ToNextStateTransitionEnd(currentTransition);
            _statemachine.ToNextStateTransitionEnd(currentTransition);
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            _statemachine.ToNextStateTransitionRunning(currentTransition);
        }
    }

}