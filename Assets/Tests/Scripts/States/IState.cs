using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.States
{
    public class EmptyState<T> : IState<T>
    {
        Transition<T>[] _transitions;
        T _context;
        public string Name => "Empty";

        public Guid ID => Guid.Empty;

        public Transition<T>[] Transitions { get => _transitions; set => _transitions = value; }
        public T Context { set => _context = value; }
        public EmptyState()
        {
            _transitions = new Transition<T>[0];
        }
        public void OnEnter()
        {

        }

        public void OnExit()
        {

        }

        public void OnUpdate()
        {

        }
    }
    public class StateMachine<T>
    {
        HashSet<IState<T>> _states;
        IState<T> _currentState;
        T _context;
        public StateMachine()
        {
            _states = new HashSet<IState<T>>();
            _context = default;
        }

        public void AddState(IState<T> state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));
            else if (_states.Contains(state))
                return;
            state.Context = _context;
            _states.Add(state);
            if (_currentState == null)
                _currentState = state;
        }
        public void RemoveState(IState<T> state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));
            else if (!_states.Contains(state))
                return;
            state.Context = default;
            _states.Remove(state);
        }
        public void AddTransitionFor(IState<T> state, Func<bool> triggerEvent, IState<T> destinationState)
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
        void ChangeState(IState<T> nextState)
        {
            _currentState?.OnExit();
            nextState.OnEnter();
            _currentState = nextState;
        }
        IState<T> CheckTransitions()
        {
            foreach (var t in _currentState.Transitions)
            {
                if (t.TriggerEvent())
                    return t.DestinationState;
            }
            return null;
        }
        public void OnUpdate()
        {
            if (_currentState == null)
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

            _currentState.OnUpdate();


        }

    }
    public class Transition<T>
    {
        public IState<T> SourceState;
        public IState<T> DestinationState;
        public Func<bool> TriggerEvent;
    }
    public interface IState<T>
    {
        public string Name { get; }
        public Guid ID { get; }
        public Transition<T>[] Transitions { get; set; }
        public T Context { set; }
        public void OnEnter();
        public void OnUpdate();
        public void OnExit();
    }
}
