using System;

namespace Tests.States
{
    public abstract class WithCallbackPlayableState_MonoComponent<T> : PlayableState_MonoComponent<T>, IWithCallbackPlayableState<T>
    {
        class PlayableState : WithCallbackPlayableState<T>
        {
            public PlayableState(string name, float duration = 0, bool enabled = true) : base(name, duration, enabled)
            {
            }
            public override void OnEnter()
            {
                throw new System.NotImplementedException();
            }

            public override void OnExit()
            {
                throw new System.NotImplementedException();
            }

            public override void OnUpdate()
            {
                throw new System.NotImplementedException();
            }
        }
        new IWithCallbackPlayableState<T> _state;
        Action _entryAction;
        Action _updateAction;
        Action _exitAction;
        protected override IState<T> CreateInternalState()
        {
            this._state = new PlayableState(this.name, 0);
            return this._state;
        }
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
    public abstract class WithCallbackPlayableState_MonoComponent : WithCallbackPlayableState_MonoComponent<object>
    {

    }
}
