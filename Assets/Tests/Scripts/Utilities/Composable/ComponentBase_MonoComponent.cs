using System;
using Tests.Utilities.Blackboards;
using Tests.Utilities.MTrees;
using UnityEngine;

namespace Tests.Utilities.Composable
{
    public abstract class ComponentBase_MonoComponent : MonoBehaviour, IComponent
    {
        Guid _id;
        protected Blackboard blackboard;
        internal ComponentNode node;
        protected ComponentBase_MonoComponent()
        {
            _id = Guid.NewGuid();
            node = new(this);
        }
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
