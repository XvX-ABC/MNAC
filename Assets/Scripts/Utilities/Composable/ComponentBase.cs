using System;
using Blackboard = MNAC.Utilities.Blackboards.Blackboard;

namespace MNAC.Utilities.Composable
{
    public abstract class ComponentBase<T> : IComponent<T>
    {
        protected T context;
        internal ComponentNode<T> node;
        protected bool enabled;
        Guid _id;
        public Guid ID { get => _id; }
        public virtual T Context
        {
            get => context;
            set
            {
                context = value;
            }
        }
        public IComponentNode<T> Node { get => node; }
        public abstract string Name { get; }
        public virtual bool Enabled { get => enabled; set => enabled = value; }

        protected ComponentBase()
        {
            _id = Guid.NewGuid();
            node = new(this);
        }

        public virtual void Initialize(T blackboard)
        {
            this.context = blackboard;
        }
        public virtual void Dispose()
        {
            this.context = default;
        }
    }

    public abstract class ComponentBase : IComponent
    {
        protected Blackboard blackboard;
        internal ComponentNode node;
        protected bool enabled;
        Guid _id;
        public Guid ID { get => _id; }
        public virtual Blackboard Blackboard
        {
            get => blackboard;
            set
            {
                blackboard = value;
            }
        }
        public IComponentNode Node { get => node; }
        public abstract string Name { get; }
        public virtual bool Enabled { get => enabled; set => enabled = value; }

        protected ComponentBase()
        {
            _id = Guid.NewGuid();
            node = new(this);
        }

        public virtual void Initialize(Blackboard blackboard)
        {
            this.blackboard = blackboard;
        }
        public virtual void Dispose()
        {
            this.blackboard = null;
        }
    }

}
