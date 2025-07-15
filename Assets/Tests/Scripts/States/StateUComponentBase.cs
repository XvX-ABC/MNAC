using System;
using UnityEngine;

namespace Tests.States
{
    public abstract class StateUComponentBase : StateUComponentBase<object>
    {

    }
    public abstract class StateUComponentBase<T> : MonoBehaviour, IState<T>
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
        protected IState<T> state;

        public string Name => this.name;

        public T Context { get => state.Context; set => state.Context = value; }
        public bool Enabled
        {
            get => state.Enabled;
            set => state.Enabled = value;
        }

        public Guid ID => state.ID;

        public ITransition<T>[] Transitions => state.Transitions;
        protected virtual void Awake()
        {
            state = new State(this.name, this.enabled);
        }

        public void AddTransition(ITransition<T> transition)
        {
            state.AddTransition(transition);
        }

        public ITransition<T> FindTransition(IState<T> destinationState)
        {
            return state.FindTransition(destinationState);
        }

        public void RemoveTransition(IState<T> destinationState)
        {
            state.RemoveTransition(destinationState);
        }

        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnUpdate();
    }


}
