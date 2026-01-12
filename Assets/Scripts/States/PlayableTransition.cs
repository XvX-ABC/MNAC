using System;
using UnityEngine;
using Tests.Utilities.Timeline;
using Tests.Utilities.Timeline.Events.Point;
using Tests.Utilities.Timeline.Events.Range;
namespace Tests.States
{
    public class PlayableTransition<T> : StateMachineBase<IPlayableState<T>, T>.Transition, IPlayableTransition<T>
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
            AddToNextStateTransitionEvents(timeline);
            AddFromPreviousStateTransitionEvents(timeline);
            _interruptionSource = interruptionSource;
        }

        protected virtual void AddToNextStateTransitionEvents(ITimeline timeline)
        {
            timeline.StartAction += _ => sourceState.ToNextStateTransitionBegin(this);
            timeline.AddRangeEvent(0, 1, _ => { sourceState.ToNextStateTransitionRunning(this); });
            timeline.EndAction += _ => sourceState.ToNextStateTransitionEnd(this);
        }
        protected virtual void AddFromPreviousStateTransitionEvents(ITimeline timeline)
        {
            timeline.StartAction += _ => destinationState.FromPreviousStateTransitionBegin(this);
            timeline.AddRangeEvent(0, 1, _ => { destinationState.FromPreviousStateTransitionRunning(this); });
            timeline.EndAction += _ => destinationState.FromPreviousStateTransitionEnd(this);
        }
        public ITimeline Timeline => timeline;

        IReadonlyTimeline IReadonlyPlayableTransition<T>.Timeline => Timeline;


    }
}


