using Tests.Characters;
using Tests.States;
using Tests.Utilities.Blackboards;

namespace Tests.Utilities.Composable
{
    public class StateComponentNode<T> : WithCallbackPlayableState<T>, IComponent
    {
        internal Blackboard blackboard;
        internal ComponentNode node;

        public StateComponentNode(string name, float duration = 0, bool enabled = true) : base(name, duration, enabled)
        {
            node = new(this);
        }

        public Blackboard Blackboard { get => blackboard; set => blackboard = value; }

        public IComponentNode Node => node;

        public virtual void Dispose()
        {
            blackboard = null;
        }

        public virtual void Initialize(Blackboard blackboard)
        {
            this.blackboard = blackboard;
        }
    }
    public class StateComponentNode : StateComponentNode<object>, IComponent
    {
        internal Blackboard blackboard;
        internal ComponentNode node;

        public StateComponentNode(string name, float duration = 0, bool enabled = true) : base(name, duration, enabled)
        {
            node = new(this);
        }

        public Blackboard Blackboard { get => blackboard; set => blackboard = value; }

        public IComponentNode Node => node;

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
