using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tests.States
{
    public class StateMachine<T>
    {
        HashSet<IState<T>> _states;
        internal IState<T> currentState;
        internal T context;
        internal Action<IState<T>, IState<T>> stateChangedAction;
        public StateBase<T> CurrentState { get => (StateBase<T>)currentState; }
        public StateMachine()
        {
            _states = new HashSet<IState<T>>();
            context = default;
        }

        public void AddState(StateBase<T> state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));
            else if (_states.Contains(state))
                return;
            state.Context = context;
            _states.Add(state);
            if (currentState == null)
                currentState = state;
        }
        public void RemoveState(StateBase<T> state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));
            else if (!_states.Contains(state))
                return;
            state.Context = default;
            _states.Remove(state);
        }
        public void AddTransitionFor(StateBase<T> state, Func<bool> triggerEvent, StateBase<T> destinationState)
        {
            AddTransitionFor(state, new Transition<T>()
            {
                SourceState = state,
                DestinationState = destinationState,
                TriggerEvent = triggerEvent
            });
        }
        void AddTransitionsFor(IState<T> state, params (Func<bool> triggerEvent, IState<T> destinationState)[] transitions)
        {
            foreach (var (triggerEvent, destinationState) in transitions)
            {
                if (triggerEvent == null)
                    throw new ArgumentNullException(nameof(triggerEvent));
                if (destinationState == null)
                    throw new ArgumentNullException(nameof(destinationState));
                if (!_states.Contains(destinationState))
                    throw new InvalidOperationException($"Destination state {destinationState.Name} is not registered in the state machine.");
                var transition = new Transition<T>
                {
                    SourceState = state,
                    DestinationState = destinationState,
                    TriggerEvent = triggerEvent
                };
                AddTransitionFor(state, transition);
            }
        }
        void AddTransitionFor(IState<T> state, Transition<T> transition)
        {
            var transitions = state.Transitions;
            if (transitions == null)
                state.Transitions = new Transition<T>[] { transition };
            else
            {
                Array.Resize(ref transitions, transitions.Length + 1);
                transitions[^1] = transition;
                state.Transitions = transitions;
            }
        }
        void RemoveTransitionFor(IState<T> state, Transition<T> transition)
        {
            var transitions = state.Transitions;
            if (transitions == null)
                return;
            var index = Array.IndexOf(transitions, transition);
            if (index == -1)
                return;
            var lastIndex = transitions.Length - 1;
            if (lastIndex == -1)
                transitions = null;
            else
            {
                if (lastIndex != index)
                    Array.Copy(transitions, index + 1, transitions, index, lastIndex);
                Array.Resize(ref transitions, lastIndex);
                state.Transitions = transitions;
            }
        }
        void ChangeState(IState<T> nextState)
        {
            var currentState = this.currentState;
            currentState?.OnExit();
            nextState.OnEnter();
            this.currentState = nextState;
            stateChangedAction?.Invoke(currentState, nextState);
        }
        internal void ChangeStateTo(StateBase<T> nextState)
        {
            if (nextState == null)
                throw new ArgumentNullException(nameof(nextState));
            if (!_states.Contains(nextState))
                return;
            ChangeState(nextState);
        }
        IState<T> CheckTransitions()
        {
            if (currentState.Transitions == null)
                return null;
            foreach (var t in currentState.Transitions)
            {
                if (t.DestinationState.Enabled && t.TriggerEvent())
                    return t.DestinationState;
            }
            return null;
        }
        public void OnUpdate()
        {
            if (currentState == null)
            {
                foreach (var s in _states)
                {
                    ChangeState(s);
                    break;
                }
            }


            var nextState = CheckTransitions();
            if (nextState != null)
            {
                ChangeState(nextState);
            }

            currentState.OnUpdate();


        }

    }
}
