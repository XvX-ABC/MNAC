using Tests.States;

namespace Tests.Characters.Arms
{
    internal class StateComponentNode : WithCallbackPlayableState, ICharacterComponent
    {
        internal Blackboard blackboard;
        internal ComponentNode node;

        public StateComponentNode(string name, float duration = 0, bool enabled = true) : base(name, duration, enabled)
        {
            node = new(this);
        }

        public Blackboard Blackboard { get => blackboard; set => blackboard = value; }

        public ICharacterComponentNode Node => node;

        public virtual void Dispose()
        {
            blackboard = null;
        }

        public virtual void Initialize(Blackboard blackboard)
        {
            this.blackboard = blackboard;
        }
    }
}
