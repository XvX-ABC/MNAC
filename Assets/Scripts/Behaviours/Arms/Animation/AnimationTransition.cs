using System;
using Tests.Animations;
using Tests.States;
using Tests.Utilities.Timeline;
using UnityEngine.Playables;
using Tests.Utilities.Timeline.Events.Range;

namespace Tests.Behaviours.Arms.Animations
{
    internal class AnimationTransition : AnimationBlendingTransition
    {
        public AnimationTransition(
            int statusNum,
            IWithCallbackPlayableState<object> sourceState,
            IAnimationPlayableState destinationState,
            Func<bool> triggerEvent,
            Action<IPlayableState<object>, IAnimationPlayableState, float> durationEvent,
            float duration,
            float offset = 0,
            float fixedExitTime = FIXED_EXIT_TIME_INVALID_VALUE,
            InterruptionSource interruptionSource = INTERRUPTION_SOURCE_DEFAULT) : base(statusNum, sourceState, destinationState, triggerEvent, null, duration, offset, fixedExitTime, interruptionSource)
        {
            if (durationEvent != null)
            {
                timeline.AddRangeEvent(0, 1, ctx =>
                {
                    durationEvent.Invoke(sourceState, destinationState, ctx.NormalizedTime);
                });
            }
        }

        void StateCheck(IAnimationPlayableState state)
        {
            if (state.Node == null)
                throw new NullReferenceException("state.Node");
            if (state.Node.Value == null)
                throw new NullReferenceException("state.Node.Value");
        }
        protected override void Begin(TimelineContext ctx)
        {
            base.Begin(ctx);
            var state = (IAnimationPlayableState)destinationState;
            StateCheck(state);
            var p = state.Node.Value.PlayablePart;
            if (!p.IsNull())
            {
                p.SetTime(offset);
                p.Play();
            }
        }
    }
}
