using System;
using UnityEngine;
using Utilities.Timeline;
namespace Tests.States
{
    internal class BlendingTransition<T> : PlayableTransition<T>
    {
        protected internal const float FIXED_EXIT_TIME_INVALID_VALUE = -1;
        float _oldLength;
        protected float offset;
        protected float fixedExitTime;


        public BlendingTransition(
        IWithCallbackPlayableState<T> sourceState,
        IWithCallbackPlayableState<T> destinationState,
        Func<bool> triggerEvent,
        Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent,
        float duration,
        float offset = 0,
        float fixedExitTime = FIXED_EXIT_TIME_INVALID_VALUE,
        InterruptionSource interruptionSource = INTERRUPTION_SOURCE_DEFAULT
       ) : base(
            sourceState,
            destinationState,
            triggerEvent,
            durationEvent,
            duration,
            interruptionSource)
        {
            this.offset = offset < 0 ? 0 : offset;
            if (fixedExitTime != FIXED_EXIT_TIME_INVALID_VALUE)
            {
                fixedExitTime = Mathf.Clamp01(fixedExitTime);
                this.fixedExitTime = fixedExitTime;
                this.AddTriggerEvent(() => this.sourceState.Timeline.NormalizedTime >= this.fixedExitTime);
            }
            this.timeline.StartAction += Begin;


        }
        protected virtual void Begin(TimelineContext ctx)
        {
            var timeline = destinationState.Timeline;
            _oldLength = timeline.Length;
            var newLength = _oldLength - (offset + ctx.Duration);
            newLength = newLength < 0 ? 0 : newLength;

            timeline.UpdateLength(newLength);
            ((IWithCallbackPlayableState<T>)destinationState).ExitAction += ResetDestinationState;
        }
        protected virtual void ResetDestinationState()
        {
            var timeline = destinationState.Timeline;
            timeline.UpdateLength(_oldLength);
            var state = (IWithCallbackPlayableState<T>)destinationState;
            state.ExitAction -= ResetDestinationState;
        }

    }

}