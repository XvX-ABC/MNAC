using System;
using Tests.States;

namespace Tests.Characters.Locomotion.Animations
{
    internal class DescendingState : LocomotionAnimationStateBase
    {
        protected ILocomotionAnimatorDefinitions definitions;
        protected ControllerPlayable controller;
        public DescendingState(string name, ILocomotionAnimatorDefinitions definitions, ControllerPlayable controller, bool enabled = true) : base(name == null ? "descending" : $"descending_{name}", 0, enabled)
        {
            this.definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            controller.SetBool(definitions.Descending, true);
        }
        public override void OnExit()
        {
            controller.SetBool(definitions.Descending, false);
            base.OnExit();
        }
    }
}
