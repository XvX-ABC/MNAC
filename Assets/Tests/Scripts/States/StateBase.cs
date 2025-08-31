using System;

namespace Tests.States
{
    public abstract class StateBase : StateBase<object>
    {
        public StateBase(string name, bool enabled = true) : base(name, enabled)
        {
        }
    }
    public abstract class StateBase<T> : IState<T>
    {
        protected ITransition<T>[] transitions;
        protected string name;
        protected readonly Guid id;
        protected bool enabled;
        protected T context;
        protected int FindTransitionIndex(IState<T> destinationState)
        {
            var transitions = this.transitions;
            if (transitions == null)
                return -1;
            var index = Array.FindIndex(transitions, t => t.DestinationState == destinationState);
            return index;
        }
        public ITransition<T>[] Transitions { get => transitions; }
        public virtual string Name { get => name; }
        public Guid ID { get => id; }
        public virtual bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }
        public virtual T Context { get => context; set => context = value; }
        public StateBase(string name, bool enabled = true)
        {
            this.name = name;
            this.id = Guid.NewGuid();
            this.enabled = enabled;
        }


        public virtual void AddTransition(ITransition<T> transition)
        {
            if (transition == null)
                throw new ArgumentNullException(nameof(transition), "Transition cannot be null.");
            var index = FindTransitionIndex(transition.DestinationState);
            if (index > -1)
                return;
            var transitions = this.transitions;
            if (transitions == null)
            {
                transitions = new ITransition<T>[] { transition };
            }
            else
            {
                Array.Resize(ref transitions, transitions.Length + 1);
                transitions[^1] = transition;
            }
            this.transitions = transitions;
        }
        public virtual void RemoveTransition(ITransition<T> transition)
        {
            RemoveTransition(transition.DestinationState);
        }
        public virtual void RemoveTransition(IState<T> destinationState)
        {
            if (destinationState == null)
                throw new ArgumentNullException(nameof(destinationState), "Destination state cannot be null.");
            var transitions = this.transitions;
            if (transitions == null || transitions.Length == 0)
                return;
            int index = Array.FindIndex(transitions, t => t.DestinationState == destinationState);
            if (index == -1)
                return;
            if (transitions.Length == 1)
                transitions = null;
            else
            {
                if (transitions.Length != index)
                    Array.Copy(transitions, index + 1, transitions, index, transitions.Length - index - 1);
                Array.Resize(ref transitions, transitions.Length - 1);
            }
            this.transitions = transitions;
        }
        public ITransition<T> FindTransition(IState<T> destinationState)
        {
            if (destinationState == null)
                return null;
            var index = FindTransitionIndex(destinationState);
            if (index > -1)
                return transitions[index];
            return null;
        }
        public abstract void OnEnter();
        public abstract void OnUpdate();
        public abstract void OnExit();
    }


}
