using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests.States
{
    // TODO：完善状态运行时的错误捕获
    public abstract class StateMachineBase<S, T> : StateBase<T> where S : class, IState<T>
    {
        internal class StateNotExistException : Exception
        {
            public StateNotExistException(string message) : base(message)
            {
            }
            public StateNotExistException(IState<T> state) : this($"The state '{state.Name}' is not exist.")
            {

            }
        }
        internal class StateEntryException : Exception
        {
            public StateEntryException(string message) : base(message)
            {

            }
            public StateEntryException(IState<T> state, Exception e, string stateType = "state") : base($"There has a exception when enter to the '{stateType}' '{state.Name}'. \n{e.Message}.")
            {
            }
        }
        internal class StateExitException : Exception
        {
            public StateExitException(string message) : base(message)
            {
            }
            public StateExitException(IState<T> state, Exception e, string stateType = "state") : base($"There has a exception when exit from the '{stateType}' '{state.Name}'. \n{e.Message}.")
            {
            }
        }
        internal class StateUpdateException : Exception
        {
            public StateUpdateException(string message) : base(message)
            {
            }
            public StateUpdateException(IState<T> state, Exception e) : base($"There has a exception when the state '{state.Name}' update.\n{e.Message}")
            {
            }
        }
        protected internal class Transition : ITransition<T>
        {
            internal S sourceState;
            internal S destinationState;
            protected Action<T> triggeredEvent;

            internal List<Func<bool>> triggerEvents;
            protected internal Transition()
            {
                triggerEvents = new();
            }
            public Transition(S sourceState, S destinationState, Func<bool> triggerEvent)
            {
                this.sourceState = sourceState ?? throw new ArgumentNullException(nameof(sourceState));
                this.destinationState = destinationState ?? throw new ArgumentNullException(nameof(destinationState));
                triggerEvents = new();
                this.AddTriggerEvent(triggerEvent);
            }

            public IState<T> SourceState => sourceState;

            public IState<T> DestinationState => destinationState;
            public Action<T> TriggeredEvent { get => triggeredEvent; set => triggeredEvent = value; }

            public bool Triggered => IsTriggered();

            public IReadOnlyList<Func<bool>> TriggerEvents => triggerEvents;

            protected bool IsTriggered()
            {
                var triggered = true;
                foreach (var evt in triggerEvents)
                {
                    triggered &= evt();
                }
                return triggered;
            }
            public override int GetHashCode()
            {
                return HashCode.Combine(sourceState, destinationState, triggerEvents);
            }
            public bool Contains(Func<bool> triggerEvent)
            {
                return triggerEvents.Contains(triggerEvent);
            }
            public void AddTriggerEvent(Func<bool> triggerEvent)
            {
                if (triggerEvent == null)
                    return;
                triggerEvents.Add(triggerEvent);
            }

            public void RemoveTriggerEvent(Func<bool> triggerEvent)
            {
                if (triggerEvent == null) return;
                triggerEvents.Remove(triggerEvent);
            }
            public void ClearTriggerEvents()
            {
                triggerEvents.Clear();
            }
        }
        protected HashSet<S> states;
        protected S currentState;
        public override T Context
        {
            get
            {
                if (currentState == null)
                    return default;
                return currentState.Context;
            }
            set
            {
                foreach (var state in states)
                {
                    state.Context = value;
                }
            }
        }
        public IState<T> CurrentState { get => currentState; }
        public StateMachineBase(string name, bool enabled = true) : base(name, enabled)
        {
            states = new();
        }
        protected virtual Transition NewTransition(S sourceState, S destinationState, Func<bool> triggerEvent)
        {
            return new Transition(sourceState, destinationState, triggerEvent);
        }
        public void AddState(S state)
        {
            if (states.Contains(state))
                return;
            state.Context = context;
            states.Add(state);

        }
        public void RemoveState(S state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));
            else if (!states.Contains(state))
                return;
            state.Context = default;
            states.Remove(state);
        }
        public void AddTransitionFor(ITransition<T> transition)
        {
            if (transition == null)
                throw new ArgumentNullException(nameof(transition));
            var srcState = transition.SourceState;
            var desState = transition.DestinationState;
            if (!states.Contains(srcState))
                throw new StateNotExistException(srcState);
            else if (!states.Contains(desState))
                throw new StateNotExistException(desState);
            srcState.AddTransition(transition);
        }
        public virtual void AddTransitionFor(S state, S destinationState, Func<bool> triggerEvent = null)
        {

            var transition = NewTransition(state, destinationState, triggerEvent);
            AddTransitionFor(transition);
        }
        public virtual void AddTransitionFor(S state, S destinationState)
        {
            AddTransitionFor(state, destinationState, null);
        }
        public void RemoveTransitionFor(ITransition<T> transition)
        {
            var srcState = transition.SourceState;
            var desState = transition.DestinationState;
            if (!states.Contains(srcState))
                throw new StateNotExistException(srcState);
            else if (!states.Contains(desState))
                throw new StateNotExistException(desState);
            srcState.RemoveTransition(desState);
        }
        protected virtual void ChangeState(IState<T> state)
        {
            if (state is not S es)
                throw new InvalidCastException($"The state type '{state.GetType().Name}' can't converts to the state type '{typeof(S)}'");
            ChangeState(es);
        }
        protected virtual void ChangeState(S nextState)
        {
            var currentState = this.currentState;
            if (currentState != null)
            {
                try
                {
                    currentState.OnExit();
                }
                catch (Exception e)
                {
                    //throw new StateExitException(currentState, e, "currentState");
                    throw;
                }
            }

            try
            {
                nextState.OnEnter();
            }
            catch (Exception e)
            {

                //throw new StateEntryException(nextState, e, "nextState");
                throw;
            }
            this.currentState = nextState;
        }
        protected virtual void ChangeState(ITransition<T> triggeredTransition)
        {
            ChangeState(triggeredTransition.DestinationState);
        }
        public virtual void ChangeStateTo(S state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));
            if (!states.Contains(state))
                throw new StateNotExistException(state);
            if (currentState != null && currentState == state)
                return;
            ChangeState(state);
        }
        protected virtual ITransition<T> CheckTransitions()
        {
            var currentState = this.currentState;
            var transitions = currentState.Transitions;
            if (transitions == null)
                return null;
            foreach (var t in transitions)
            {
                if (t.DestinationState.Enabled)
                {
                    //if (t.TriggerEvent == null)
                    //    return t;
                    //else if (t.TriggerEvent())
                    //{
                    //    t.TriggeredEvent?.Invoke(context);
                    //    return t;
                    //}

                    if (t.TriggerEvents.Count == 0)
                        return t;
                    else if (t.Triggered)
                    {
                        t.TriggeredEvent?.Invoke(context);
                        return t;
                    }
                }

            }
            return null;
        }
        public override void OnUpdate()
        {
            if (!this.enabled)
                return;
            if (currentState == null && states.Count > 0)
            {
                var state = states.First();
                //state.OnEnter();
                //currentState = state;

                ChangeState(state);
            }
            var currentTransition = CheckTransitions();
            if (currentTransition != null)
            {
                ChangeState(currentTransition);
            }
            currentState?.OnUpdate();
        }
        public override void OnEnter()
        {
            this.enabled = true;
            currentState?.OnEnter();
        }
        public override void OnExit()
        {
            currentState?.OnExit();
            this.enabled = false;
        }
        public override string ToString()
        {
            if (!this.enabled)
                return "This state machine is not enabled.";
            var sb = new StringBuilder();
            sb.AppendLine("current state: " + currentState.Name);
            return sb.ToString();
        }
    }
}
