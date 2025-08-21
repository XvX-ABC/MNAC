using System;
using Tests.Characters;
using Tests.Characters.Arms;
using Tests.States;

namespace Tests.Behaviours.Arms.Weapons
{
    [Obsolete]
    public abstract class ArmPlayableState_Obsolete : PlayableStateBase, IArmBehaviour, IWithCallbackPlayableState<object>
    {
        protected ArmPlayableState_Obsolete(string name, float duration = 0, bool enabled = true) : base($"arm_{name}", duration, enabled)
        {
            node = new(this);
        }
        Action _entryAction;
        Action _updateAction;
        Action _exitAction;
        internal ComponentNode node;
        protected Blackboard blackboard;
        public Action EntryAction { get => _entryAction; set => _entryAction = value; }
        public Action UpdateAction { get => _updateAction; set => _updateAction = value; }
        public Action ExitAction { get => _exitAction; set => _exitAction = value; }
        public virtual Blackboard Blackboard
        {
            get => blackboard;
            set
            {
                blackboard = value;
            }
        }

        public ICharacterComponentNode Node => node;

        public override void OnEnter()
        {
            _entryAction?.Invoke();
        }
        public override void OnExit()
        {
            _exitAction?.Invoke();
        }
        public override void OnUpdate()
        {
            _updateAction?.Invoke();
        }

        public virtual void Initialize(Blackboard blackboard)
        {
            this.blackboard = blackboard;
        }
        public virtual void Dispose()
        {
            blackboard = null;
        }
    }
}
