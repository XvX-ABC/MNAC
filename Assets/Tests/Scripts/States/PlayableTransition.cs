using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Range;
using System;

namespace Tests.States
{
    public partial class PlayableStateMachine<T>
    {
        protected internal class PlayableTransition : Transition, IPlayableTransition<T>
        {
            ITimeline _timeline;
            protected internal PlayableTransition() : base() { }
            public PlayableTransition(IPlayableState<T> sourceState, IPlayableState<T> destinationState, Func<bool> triggerEvent, Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent, float duration) : base(sourceState, destinationState, triggerEvent)
            {
                _timeline = new Timeline(duration);
                if (durationEvent != null)
                    _timeline.AddRangeEvent(0, 1, ctx =>
                    {
                        durationEvent.Invoke(sourceState, destinationState, ctx.Proportion);
                    });
                _timeline.AddRangeEvent(0, 1, ctx => { sourceState.OnTransitionWhichToNextState(this); });
                _timeline.AddRangeEvent(0, 1, ctx => { destinationState.OnTransitionWhichOfPreviousState(this); });
            }

            public ITimeline Timeline => _timeline;

            IReadonlyTimeline IReadonlyPlayableTransition<T>.Timeline => Timeline;
        }
    }

}
