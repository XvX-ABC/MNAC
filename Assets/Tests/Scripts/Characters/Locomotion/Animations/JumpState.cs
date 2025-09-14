using System;
using Tests.States;
using Utilities.Timeline;

namespace Tests.Characters.Locomotion.Animations
{
    internal class JumpState : LocomotionAnimationStateBase
    {
        protected ILocomotionAnimatorDefinitions definitions;
        protected ControllerPlayable controller;
        public JumpState(ILocomotionAnimatorDefinitions definitions, ControllerPlayable controller, ITimeline jumpTimeline, bool enabled = true) : base("jump", 0,  enabled)
        {
            this.definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
            this.timeline = jumpTimeline ?? throw new ArgumentNullException(nameof(jumpTimeline));
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            var clipLength = definitions.Jump_Clip_Length;
            var m = timeline.Length / clipLength <= 0 ? 1 : clipLength;
            controller.SetFloat(definitions.Jump_Multiplier, m);
            controller.SetTrigger(definitions.Jump_Trigger);
        }
    }
}
