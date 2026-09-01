using System;
using UnityEngine;

namespace MNAC.States
{
    public class PlayableStatemachineState<T> : WithCallbackPlayableState<T>
    {
        PlayableStateMachine<T> _statmachine;
        public PlayableStatemachineState(PlayableStateMachine<T> statemachine, string name, float duration = 0, bool enabled = true) : base(name, duration, enabled)
        {
            _statmachine = statemachine ?? throw new ArgumentNullException(nameof(statemachine));
        }

        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            _statmachine.FromPreviousStateTransitionBegin(currentTransition);
        }

        public override void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.FromPreviousStateTransitionEnd(currentTransition);
            _statmachine.FromPreviousStateTransitionEnd(currentTransition);
        }

        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            _statmachine.FromPreviousStateTransitionRunning(currentTransition);
        }

        public override void OnEnter()
        {
            _statmachine.OnEnter();
        }

        public override void OnExit()
        {
            _statmachine.OnExit();
        }

        public override void OnUpdate()
        {
            _statmachine.OnUpdate();
        }

        public override void ToNextStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.ToNextStateTransitionBegin(currentTransition);
            _statmachine.ToNextStateTransitionBegin(currentTransition);
        }

        public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.ToNextStateTransitionEnd(currentTransition);
            _statmachine.ToNextStateTransitionEnd(currentTransition);
        }

        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            _statmachine.ToNextStateTransitionRunning(currentTransition);
        }
    }
}
