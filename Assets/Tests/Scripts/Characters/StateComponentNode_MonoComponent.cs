using Tests.Characters.Arms;
using Tests.States;

namespace Tests.Characters
{
    public class StateComponentNode_MonoComponent : WithCallbackPlayableState_MonoComponent, ICharacterComponent
    {
        internal Blackboard blackboard;
        internal ComponentNode node;
        protected override void Awake()
        {
            base.Awake();
            node = new(this);
        }
        protected override IState<object> CreateInternalState()
        {
            return new StateComponentNode(name, 0);
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
