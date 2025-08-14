using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using Assets.Scripts.Utilities.Timeline.Event.Range;
using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;

namespace Tests.States
{
    internal class PlayableTransition<T> : StateMachineBase<IPlayableState<T>, T>.Transition, IPlayableTransition<T>
    {
        public const byte INTERRUPTION_SOURCE_NEXT_CODE = 1;
        protected ITimeline timeline;
        byte _interruptionSource;
        public byte InterruptionSource { get => _interruptionSource; }
        protected internal PlayableTransition() : base() { }
        public PlayableTransition(IPlayableState<T> sourceState, IPlayableState<T> destinationState, Func<bool> triggerEvent, Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent, float duration, byte interruptionSource) : base(sourceState, destinationState, triggerEvent)
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


