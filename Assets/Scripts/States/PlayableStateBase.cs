using System;
using MNAC.Utilities.Timeline;

namespace MNAC.States
{
    public abstract class PlayableStateBase : PlayableStateBase<object>
    {
        public PlayableStateBase(string name, float duration = 0, bool enabled = true) : base(name, duration, enabled)
        {
        }
    }
    public abstract class PlayableStateBase<T> : StateBase<T>, IPlayableState<T>
    {
        protected ITimeline timeline;
        bool _exitWhenEnd;
        IPlayableTransition<T>[] _transitions;
        public PlayableStateBase(string name, float duration = 0, bool enabled = true) : base(name, enabled)
        {
            timeline = NewTimeline(duration);
        }
        protected virtual ITimeline NewTimeline(float duration)
        {
            return new Timeline(duration);
        }
        public ITimeline Timeline { get => timeline; }

        public bool ExitWhenEnd
        {
            get => _exitWhenEnd;
            set => _exitWhenEnd = value;
        }

        public new IPlayableTransition<T>[] Transitions { get => _transitions; }


        public override void AddTransition(ITransition<T> transition)
        {
            if (transition is IPlayableTransition<T> pt)
            {
                base.AddTransition(pt);
                _transitions = Array.ConvertAll(base.transitions, it => (IPlayableTransition<T>)it);
            }
            else
                return;
        }


        public virtual void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition)
        {
        }

        public virtual void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition)
        {
        }
        public virtual void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition)
        {
        }

        public virtual void ToNextStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition)
        {
        }
        public virtual void ToNextStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition)
        {
        }
        public virtual void ToNextStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition)
        {
        }
    }

}
