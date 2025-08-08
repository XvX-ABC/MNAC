using Assets.Scripts.Utilities.Timeline;
using System;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Playables;

namespace Tests.States
{
    public abstract class PlayableStateBase : PlayableStateBase<object>
    {
        protected PlayableStateBase(string name, float duration = 0, bool enabled = true) : base(name, duration, enabled)
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
                //_transitions = (IPlayableTransition<T>[])base.transitions;
                _transitions = Array.ConvertAll(base.transitions, it => (IPlayableTransition<T>)it);
            }
            else
                return;
        }

        public void OnTransitionRunning(IPlayableTransition<T> transition)
        {
            if (transition is IPlayableTransition<T> pt)
            {
                base.RemoveTransition((IState<T>)pt);
                _transitions = Array.ConvertAll(base.transitions, it => (IPlayableTransition<T>)it);
            }
            else
                return;
        }




        public virtual void TransitionBeginWhichOfPreviousState(IReadonlyPlayableTransition<T> currentTransition)
        {
        }

        public virtual void TransitionRunningWhichOfPreviousState(IReadonlyPlayableTransition<T> currentTransition)
        {
        }
        public virtual void TransitionEndWhichOfPreviousState(IReadonlyPlayableTransition<T> currentTransition)
        {
        }

        public virtual void TransitionBeginWhichToNextState(IReadonlyPlayableTransition<T> currentTransition)
        {
        }
        public virtual void TransitionRunningWhichToNextState(IReadonlyPlayableTransition<T> currentTransition)
        {
        }
        public virtual void TransitionEndWhichToNextState(IReadonlyPlayableTransition<T> currentTransition)
        {
        }
    }

}
