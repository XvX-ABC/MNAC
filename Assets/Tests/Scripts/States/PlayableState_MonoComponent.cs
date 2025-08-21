using Utilities.Timeline;

namespace Tests.States
{
    public abstract class PlayableState_MonoComponent : PlayableState_MonoComponent<object>
    {
    }
    public abstract class PlayableState_MonoComponent<T> : State_MonoComponent<T>, IPlayableState<T>
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
            this._state = new PlayableState(this.name, 0, this.enabled);
            return this._state;
        }

        public virtual void TransitionRunningWhichOfPreviousState(IReadonlyPlayableTransition<T> currentTransition)
        {
        }

        public virtual void TransitionRunningWhichToNextState(IReadonlyPlayableTransition<T> currentTransition)
        {
        }

        public virtual void TransitionBeginWhichOfPreviousState(IReadonlyPlayableTransition<T> currentTransition)
        {

        }

        public virtual void TransitionEndWhichOfPreviousState(IReadonlyPlayableTransition<T> currentTransition)
        {

        }

        public virtual void TransitionBeginWhichToNextState(IReadonlyPlayableTransition<T> currentTransition)
        {

        }

        public virtual void TransitionEndWhichToNextState(IReadonlyPlayableTransition<T> currentTransition)
        {

        }
    }

}
