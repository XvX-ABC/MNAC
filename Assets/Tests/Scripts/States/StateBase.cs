using System;

namespace Tests.States
{
    public abstract class StateBase : StateBase<object>
    {
        protected StateBase(string name) : base(name)
        {
        }
    }
    public abstract class StateBase<T> : IState<T>
    {
        Transition<T>[] _transitions;
        protected string name;
        protected readonly Guid id;
        protected bool enabled;
        protected T context;

        public string Name { get => name; }
        public Guid ID { get => id; }
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }
        public T Context { set => context = value; }
        Transition<T>[] IState<T>.Transitions { get => _transitions; set => _transitions = value; }
        protected StateBase(string name, bool enabled = true)
        {
            this.id = Guid.NewGuid();
            this.enabled = enabled;
        }
        public virtual void OnEnter() { }
        public virtual void OnUpdate() { }
        public virtual void OnExit() { }
    }
}
