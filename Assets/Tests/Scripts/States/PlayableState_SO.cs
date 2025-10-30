using Utilities.Timeline;

namespace Tests.States
{
    public abstract class PlayableState_SO<T> : State_SO<T>, IPlayableState<T>
    {

        class PlayableState : PlayableStateBase<T>
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

        public ITimeline Timeline => _state.Timeline;

        public bool ExitWhenEnd { get => _state.ExitWhenEnd; set => _state.ExitWhenEnd = value; }
        public new IPlayableTransition<T>[] Transitions => _state.Transitions;
        protected IPlayableState<T> _state;
        protected override IState<T> CreateInternalState()
        {
            this._state = new PlayableState(this.name, 0, this.Enabled);
            return this._state;
        }

        public virtual void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition)
        {
        }

        public virtual void ToNextStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition)
        {
        }

        public virtual void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition)
        {

        }

        public virtual void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition)
        {

        }

        public virtual void ToNextStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition)
        {

        }

        public virtual void ToNextStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition)
        {

        }
    }

}
