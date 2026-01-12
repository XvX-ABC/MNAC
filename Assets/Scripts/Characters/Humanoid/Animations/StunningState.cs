using System;
using Tests.Animations;
using Tests.Characters.Animations;
using Tests.States;
using Tests.Utilities.Timeline;
namespace Tests.Characters.Humanoid.Animations
{
    internal class StunningState : HumanAnimationStateBase
    {
        ControllerPlayable _controller;
        IStunningAnimationDefinitions _definitions;
        public StunningState(ITimeline timeline, ControllerPlayable controller, IStunningAnimationDefinitions definitions, bool enabled = true) : base("stunning", 0, enabled)
        {
            this.timeline = timeline ?? throw new ArgumentNullException(nameof(timeline));
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _definitions = definitions;
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            var clipLength = _definitions.ClipLength;
            var m = clipLength / (timeline.Length <= 0 ? 1 : timeline.Length);
            _controller.SetFloat(_definitions.Multiplier, m);
            _controller.SetTrigger(_definitions.Trigger);

        }
    }
}
