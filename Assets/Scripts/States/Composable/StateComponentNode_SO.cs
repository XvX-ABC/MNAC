using Tests.States;
using Tests.Utilities.Blackboards;

namespace Tests.Utilities.Composable
{
    public class StateComponentNode_SO : WithCallbackPlayableState_SO, IComponent
    {
        internal protected Blackboard blackboard;
        internal ComponentNode node;
        protected override void OnEnable()
        {
            base.OnEnable();
            node = new(this);
        }
        protected override IState<object> CreateInternalState()
        {
            return new StateComponentNode(name, 0);
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
