using System;
using Tests.Utilities.Timeline;

namespace Tests.States
{
    public interface IPlayableState<T> : IState<T>
    {
        public ITimeline Timeline { get; }
        [Obsolete]
        public bool ExitWhenEnd { get; set; }
        public new IPlayableTransition<T>[] Transitions { get; }
        public void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition);
        public void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition);
        public void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition);
        public void ToNextStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition);
        public void ToNextStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition);
        public void ToNextStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition);
    }
}
