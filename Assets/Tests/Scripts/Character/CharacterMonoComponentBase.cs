using System;
using Tests.Utilities.MTrees;

namespace Tests.Characters
{
    public abstract class CharacterMonoComponentBase : ICharacterComponent
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
                if (blackboard != null)
                    UnregisterInBlackboard(blackboard);
                if (value != null)
                    RegisterToBlackboard(value);
                blackboard = value;
            }
        }
        protected virtual void RegisterToBlackboard(Blackboard blackboard)
        {
            blackboard.TryRegisterField(this.Name, this);
        }
        protected virtual void UnregisterInBlackboard(Blackboard blackboard)
        {
            blackboard.TryUnregisterField(this.Name);
        }
        public abstract string Name { get; }
        protected virtual void Awake()
        {
            _id = Guid.NewGuid();
            node = new(_id, this);
        }
    }

}
