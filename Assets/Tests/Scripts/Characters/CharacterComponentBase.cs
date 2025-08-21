using System;
using Tests.Utilities.MTrees;

namespace Tests.Characters
{
    public abstract class CharacterComponentBase : ICharacterComponent
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
        public ICharacterComponentNode Node { get => node; }
        public abstract string Name { get; }
        public bool Enabled { get => enabled; set => enabled = value; }

        protected CharacterComponentBase()
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
