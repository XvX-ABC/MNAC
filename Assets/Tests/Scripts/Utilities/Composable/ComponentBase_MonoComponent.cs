using System;
using Tests.Utilities.Attributes;
using Tests.Utilities.Blackboards;
using UnityEngine;

namespace Tests.Utilities.Composable
{
    public abstract class ComponentBase_MonoComponent<T> : MonoBehaviour, IComponent<T>
    {
        Guid _id;
        protected T context;
        internal ComponentNode<T> node;
        //protected ComponentBase_MonoComponent()
        //{
        //    _id = Guid.NewGuid();
        //    node = new(this);
        //}
        public Guid ID { get => _id; }
        public IComponentNode<T> Node { get => node; }
        public virtual T Context
        {
            get => context;
            set
            {
                context = value;
            }
        }
        public virtual string Name { get => this.name; }
        public virtual bool Enabled { get => enabled; set => enabled = value; }
        protected virtual void Awake()
        {
            _id = Guid.NewGuid();
            node = new(this);
            AttributeProcessingCore.Process(this);
        }

        public virtual void Initialize(T context)
        {
            this.context = context;
        }
        public virtual void Dispose()
        {
            if (node.Children.Count > 0)
            {
                foreach (var node in node.Children)
                    ((IComponentNode)node).Value.Dispose();
            }
            this.node.Children.Clear();
            this.context = default;
        }
    }
    public abstract class ComponentBase_MonoComponent : MonoBehaviour, IComponent
    {
        Guid _id;
        protected Blackboard blackboard;
        protected internal ComponentNode node;
        //protected ComponentBase_MonoComponent()
        //{
        //    _id = Guid.NewGuid();
        //    node = new(this);
        //}
        public Guid ID { get => _id; }
        public IComponentNode Node { get => node; }
        public virtual Blackboard Blackboard
        {
            get => blackboard;
            set
            {
                blackboard = value;
            }
        }
        public virtual string Name { get => this.name; }
        public virtual bool Enabled { get => enabled; set => enabled = value; }
        protected virtual void Awake()
        {
            _id = Guid.NewGuid();
            node = new(this);
            AttributeProcessingCore.Process(this);
        }

        public virtual void Initialize(Blackboard blackboard)
        {
            this.blackboard = blackboard;
        }
        public virtual void Dispose()
        {
            if (node.Children.Count > 0)
            {
                foreach (var node in node.Children)
                    ((IComponentNode)node).Value.Dispose();
            }
            this.node.Children.Clear();
            this.blackboard = null;
        }
    }

}
