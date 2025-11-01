using System;

namespace Tests.States
{
    public abstract class WithCallbackPlayableState : WithCallbackPlayableState<object>
    {
        public WithCallbackPlayableState(string name, float duration = 0, bool enabled = true) : base(name, duration, enabled)
        {
        }
    }
    public abstract class WithCallbackPlayableState<T> : PlayableStateBase<T>, IWithCallbackPlayableState<T>
    {
        public WithCallbackPlayableState(string name, float duration = 0, bool enabled = true) : base(name, duration, enabled)
        {
        }
        Action _entryAction;
        Action _updateAction;
        Action _exitAction;
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
