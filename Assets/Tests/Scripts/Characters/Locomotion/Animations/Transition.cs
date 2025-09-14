using System;
using Tests.States;

namespace Tests.Characters.Locomotion.Animations
{
    internal class Transition : BlendingTransition<object>
    {
        public Transition(IWithCallbackPlayableState<object> sourceState, IWithCallbackPlayableState<object> destinationState, Func<bool> triggerEvent, Action<IPlayableState<object>, IPlayableState<object>, float> durationEvent, float duration, float offset = 0, float fixedExitTime = FIXED_EXIT_TIME_INVALID_VALUE, InterruptionSource interruptionSource = InterruptionSource.Next) : base(sourceState, destinationState, triggerEvent, durationEvent, duration, offset, fixedExitTime, interruptionSource)
        {
        }
    }
}
