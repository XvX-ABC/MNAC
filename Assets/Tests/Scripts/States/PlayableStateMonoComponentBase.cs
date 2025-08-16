using Assets.Scripts.Utilities.Timeline;

namespace Tests.States
{
    public abstract class PlayableStateMonoComponentBase : PlayableStateMonoComponentBase<object>
    {
    }
    public abstract class PlayableStateMonoComponentBase<T> : StateMonoComponentBase<T>, IPlayableState<T>
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

        public ITimeline Timeline => state.Timeline;

        public bool ExitWhenEnd { get => state.ExitWhenEnd; set => state.ExitWhenEnd = value; }
        public new IPlayableTransition<T>[] Transitions => state.Transitions;
        protected new IPlayableState<T> state;
        protected override void Awake()
        {
            this.state = new PlayableState(this.name, 0, this.enabled);
            base.state = this.state;

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
