using System;
using MNAC.Animations;
using MNAC.Characters.Animations;
using MNAC.States;
using MNAC.Utilities.Timeline;
namespace MNAC.Characters.Humanoid.Animations
{
    internal class DeathState : HumanAnimationStateBase
    {
        ControllerPlayable _controller;
        IDeathAnimationDefinitions _definitions;
        public DeathState(ITimeline timeline, ControllerPlayable controller, IDeathAnimationDefinitions definitions, bool enabled = true) : base("death", 0, enabled)
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
            //_controller.SetTrigger(_definitions.Trigger);
            _controller.SetBool(_definitions.Trigger, true);
        }
        public override void OnExit()
        {
            _controller.SetBool(_definitions.Trigger, false);
        }
    }
}
