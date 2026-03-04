using System;
using UnityEngine;

namespace MNAC.States
{
    public abstract class State_SO : State_SO<object>
    {

    }
    public abstract class State_SO<T> : ScriptableObject, IState<T>
    {
        class State : StateBase<T>
        {
            public State(string name, bool enabled = true) : base(name, enabled)
            {
            }

            public override void OnEnter()
            {
                throw new NotImplementedException();
            }

            public override void OnExit()
            {
                throw new NotImplementedException();
            }

            public override void OnUpdate()
            {
                throw new NotImplementedException();
            }
        }

        IState<T> _state;

        public string Name { get; private set; }

        public T Context
        {
            get => _state.Context;
            set => _state.Context = value;
        }
        public bool Enabled
        {
            get => _state.Enabled;
            set => _state.Enabled = value;
        }

        public Guid ID => _state.ID;

        public virtual ITransition<T>[] Transitions => _state.Transitions;
        protected virtual void OnEnable()
        {
            _state = CreateInternalState();
        }

        protected virtual IState<T> CreateInternalState()
        {
            return new State(this.name, this.Enabled);
        }

        public void AddTransition(ITransition<T> transition)
        {
            _state.AddTransition(transition);
        }

        public ITransition<T> FindTransition(IState<T> destinationState)
        {
            return _state.FindTransition(destinationState);
        }

        public void RemoveTransition(IState<T> destinationState)
        {
            _state.RemoveTransition(destinationState);
        }

        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnUpdate();
    }
}


