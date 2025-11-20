using System;
using Tests.States;

namespace Tests.Weapons_New.Launcher
{
    internal class Transition : BlendingTransition<object>
    {
        public Transition(IWithCallbackPlayableState<object> sourceState, IWithCallbackPlayableState<object> destinationState, Func<bool> triggerEvent, Action<IPlayableState<object>, IPlayableState<object>, float> durationEvent, BlendingTransitionOptions options) : base(sourceState, destinationState, triggerEvent, durationEvent, options)
        {
        }

        public Transition(IWithCallbackPlayableState<object> sourceState, IWithCallbackPlayableState<object> destinationState, Func<bool> triggerEvent, Action<IPlayableState<object>, IPlayableState<object>, float> durationEvent, float duration, float offset = 0, float fixedExitTime = -1, InterruptionSource interruptionSource = InterruptionSource.Next) : base(sourceState, destinationState, triggerEvent, durationEvent, duration, offset, fixedExitTime, interruptionSource)
        {
        }
    }
}
