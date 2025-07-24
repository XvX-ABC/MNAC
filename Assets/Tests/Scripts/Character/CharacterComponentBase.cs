using System;
using Tests.Utilities.MTrees;

namespace Tests.Characters
{
    public abstract class CharacterComponentBase : ICharacterComponent
    {
        protected Blackboard blackboard;
        internal ComponentNode node;
        Guid _id;
        public Guid ID { get => _id; }
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
        public ICharacterComponentNode Node { get => node; }
        protected virtual void RegisterToBlackboard(Blackboard blackboard)
        {
            blackboard.TryRegisterField(this.Name, this);
        }
        protected virtual void UnregisterInBlackboard(Blackboard blackboard)
        {
            blackboard.TryUnregisterField(this.Name);
        }
        public abstract string Name { get; }
        protected CharacterComponentBase()
        {
            _id = Guid.NewGuid();
            node = new(_id, this);
        }
    }

}
