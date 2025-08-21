using System;
using Utilities.Timeline;

namespace Tests.States
{
    public interface IPlayableState<T> : IState<T>
    {
        public ITimeline Timeline { get; }
        [Obsolete]
        public bool ExitWhenEnd { get; set; }
        public new IPlayableTransition<T>[] Transitions { get; }
        public void TransitionBeginWhichOfPreviousState(IReadonlyPlayableTransition<T> currentTransition);
        public void TransitionRunningWhichOfPreviousState(IReadonlyPlayableTransition<T> currentTransition);
        public void TransitionEndWhichOfPreviousState(IReadonlyPlayableTransition<T> currentTransition);
        public void TransitionBeginWhichToNextState(IReadonlyPlayableTransition<T> currentTransition);
        public void TransitionRunningWhichToNextState(IReadonlyPlayableTransition<T> currentTransition);
        public void TransitionEndWhichToNextState(IReadonlyPlayableTransition<T> currentTransition);
    }
}
