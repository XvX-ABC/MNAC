using System;
using Tests.States;
using Tests.Utilities.Timeline;

namespace Tests.Characters.Humanoid.Locomotion.Animations
{
    internal class BindLocomotionTransition : BlendingTransition<object>
    {
        public BindLocomotionTransition(IWithCallbackPlayableState<object> sourceState, IWithCallbackPlayableState<object> destinationState, Func<bool> triggerEvent, Action<IPlayableState<object>, IPlayableState<object>, float> durationEvent, float duration, float offset = 0, float fixedExitTime = FIXED_EXIT_TIME_INVALID_VALUE, InterruptionSource interruptionSource = InterruptionSource.Next) : base(sourceState, destinationState, triggerEvent, durationEvent, duration, offset, fixedExitTime, interruptionSource)
        {
        }
        protected override void Begin(TimelineContext ctx)
        {
        }
        protected override void ResetDestinationState()
        {
        }
    }
}
