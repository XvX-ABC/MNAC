using System;
using Tests.States;
using Utilities.Timeline;

namespace Tests.Behaviours.Arms.Animations
{
    internal class AnimationBlendingTransition : BlendingTransition<object>
    {
        //internal ArmAnimationCore animationCore;
        internal ArmAnimationCore animationCore;
        int _statusNum;
        public AnimationBlendingTransition(
            int statusNum,
           IWithCallbackPlayableState<object> sourceState,
           IWithCallbackPlayableState<object> destinationState,
           Func<bool> triggerEvent,
           Action<IPlayableState<object>, IPlayableState<object>, float> durationEvent,
           float duration,
           float offset = 0,
           float fixedExitTime = FIXED_EXIT_TIME_INVALID_VALUE,
           InterruptionSource interruptionSource = INTERRUPTION_SOURCE_DEFAULT) : base(sourceState, destinationState, triggerEvent, durationEvent, duration, offset, fixedExitTime, interruptionSource)
        {
            _statusNum = statusNum;
        }
        protected override void Begin(TimelineContext ctx)
        {
            base.Begin(ctx);
            if (animationCore == null)
                return;
            animationCore.StatusNum = (byte)_statusNum;
        }
    }
}
