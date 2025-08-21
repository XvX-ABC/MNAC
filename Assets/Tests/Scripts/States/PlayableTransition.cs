using System;
using Utilities.Timeline;
using Utilities.Timeline.Events.Point;
using Utilities.Timeline.Events.Range;

namespace Tests.States
{
    internal class PlayableTransition<T> : StateMachineBase<IPlayableState<T>, T>.Transition, IPlayableTransition<T>
    {
        public const InterruptionSource INTERRUPTION_SOURCE_DEFAULT = States.InterruptionSource.Next;
        protected ITimeline timeline;
        InterruptionSource _interruptionSource;

        public InterruptionSource InterruptionSource { get => _interruptionSource; }
        protected internal PlayableTransition() : base() { }
        public PlayableTransition(IPlayableState<T> sourceState, IPlayableState<T> destinationState, Func<bool> triggerEvent, Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent, float duration, InterruptionSource interruptionSource) : base(sourceState, destinationState, triggerEvent)
        {
            timeline = new Timeline(duration);
            if (durationEvent != null)
                timeline.AddRangeEvent(0, 1, ctx =>
                {
                    durationEvent.Invoke(sourceState, destinationState, ctx.NormalizedTime);
                });
            timeline.AddPointEvent(0, _ => { sourceState.TransitionBeginWhichToNextState(this); });
            timeline.AddRangeEvent(0, 1, _ => { sourceState.TransitionRunningWhichToNextState(this); });
            timeline.AddPointEvent(1, _ => { sourceState.TransitionEndWhichToNextState(this); });


            timeline.AddPointEvent(0, _ => { destinationState.TransitionBeginWhichOfPreviousState(this); });
            timeline.AddRangeEvent(0, 1, _ => { destinationState.TransitionRunningWhichOfPreviousState(this); });
            timeline.AddPointEvent(1, _ => { destinationState.TransitionEndWhichOfPreviousState(this); });
            _interruptionSource = interruptionSource;
        }


        public ITimeline Timeline => timeline;

        IReadonlyTimeline IReadonlyPlayableTransition<T>.Timeline => Timeline;


    }
}


