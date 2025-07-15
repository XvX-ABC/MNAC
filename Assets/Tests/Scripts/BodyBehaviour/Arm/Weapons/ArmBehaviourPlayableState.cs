using System;
using Tests.Input;
using Tests.States;

namespace Tests.Behaviours.Arm
{
    public abstract class ArmBehaviourPlayableState : PlayableStateBase, IArmBehaviour
    {
        protected ArmBehaviourPlayableState(string name, float duration = 0, bool enabled = true) : base($"arm_{name}", duration, enabled)
        {
        }
        Action _entryAction;
        Action _updateAction;
        Action _exitAction;
        //public abstract IInput Input { set; }
        //public abstract bool Continuing { get; }
        public Action EntryAction { get => _entryAction; set => _entryAction = value; }
        public Action UpdateAction { get => _updateAction; set => _updateAction = value; }
        public Action ExitAction { get => _exitAction; set => _exitAction = value; }
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
    }
}
