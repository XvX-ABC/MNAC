using System;
using Tests.Utilities.MTrees;
using UnityEngine;

namespace Tests.Utilities.Composable
{
    public abstract class ComponentBase_MonoComponent : MonoBehaviour, IComponent
    {
        Guid _id;
        protected Blackboard blackboard;
        internal ComponentNode node;
        public Guid ID { get => _id; }
        public ICharacterComponentNode Node { get => node; }
        public virtual Blackboard Blackboard
        {
            get => blackboard;
            set
            {
                blackboard = value;
            }
        }
        public virtual string Name { get => this.name; }
        public bool Enabled { get => enabled; set => enabled = value; }
        protected virtual void Awake()
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
