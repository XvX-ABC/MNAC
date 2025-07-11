using System;
using UnityEngine;

namespace Tests.States
{
    public abstract class MonoStateBase : MonoStateBase<object>
    {
    }
    public abstract class MonoStateBase<T> : MonoBehaviour, IState<T>
    {
        protected T context;
        protected readonly Guid id;
        ITransition<T>[] _transitions;
        public virtual string Name => this.name;

        public T Context { get => context; set => context = value; }
        public bool Enabled { get => this.enabled; set => this.enabled = value; }

        public Guid ID => id;

        public ITransition<T>[] Transitions { get => _transitions; set => _transitions = value; }

        protected MonoStateBase()
        {
            this.id = Guid.NewGuid();
        }
        public virtual void OnEnter() { }
        public virtual void OnUpdate() { }
        public virtual void OnExit() { }

    }

}
