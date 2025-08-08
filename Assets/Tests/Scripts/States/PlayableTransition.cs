using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using Assets.Scripts.Utilities.Timeline.Event.Range;
using System;

namespace Tests.States
{
    public partial class PlayableStateMachine<T>
    {
        protected internal class PlayableTransition : Transition, IPlayableTransition<T>
        {
            ITimeline _timeline;
            byte _interruptionSource;
            public byte InterruptionSource { get => _interruptionSource; }
            protected internal PlayableTransition() : base() { }
            public PlayableTransition(IPlayableState<T> sourceState, IPlayableState<T> destinationState, Func<bool> triggerEvent, Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent, float duration, byte interruptionSource) : base(sourceState, destinationState, triggerEvent)
            {
                _timeline = new Timeline(duration);
                if (durationEvent != null)
                    _timeline.AddRangeEvent(0, 1, ctx =>
                    {
                        durationEvent.Invoke(sourceState, destinationState, ctx.Proportion);
                    });
                _timeline.AddPointEvent(0, _ => { sourceState.TransitionBeginWhichToNextState(this); });
                _timeline.AddRangeEvent(0, 1, _ => { sourceState.TransitionRunningWhichToNextState(this); });
                _timeline.AddPointEvent(1, _ => { sourceState.TransitionEndWhichToNextState(this); });


                _timeline.AddPointEvent(0, _ => { destinationState.TransitionBeginWhichOfPreviousState(this); });
                _timeline.AddRangeEvent(0, 1, _ => { destinationState.TransitionRunningWhichOfPreviousState(this); });
                _timeline.AddPointEvent(1, _ => { destinationState.TransitionEndWhichOfPreviousState(this); });
                _interruptionSource = interruptionSource;
            }

            public ITimeline Timeline => _timeline;

            IReadonlyTimeline IReadonlyPlayableTransition<T>.Timeline => Timeline;


        }
    }

}
