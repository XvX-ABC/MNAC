using System;
using System.Diagnostics.CodeAnalysis;
using Tests.Utilities.Timeline.Events.Point;
namespace Tests.States
{
    public class SubStatemachineTransition<T> : BlendingTransition<T>
    {
        public SubStatemachineTransition(
        IWithCallbackPlayableState<T> sourceState,
     WithCallbackPlayableStatemachine<T> subStatemachine,
        IWithCallbackPlayableState<T> destinationState,
        Func<bool> triggerEvent,
        Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent,
        float duration,
        float offset = 0,
        float fixedExitTime = FIXED_EXIT_TIME_INVALID_VALUE,
        InterruptionSource interruptionSource = INTERRUPTION_SOURCE_DEFAULT
       ) : base(
            sourceState,
            subStatemachine,
            triggerEvent,
            durationEvent,
            duration,
            offset,
            fixedExitTime,
            interruptionSource)
        {
            timeline.AddPointEvent(0, _ => { subStatemachine.ChangeStateTo(destinationState); });
        }
    }

}