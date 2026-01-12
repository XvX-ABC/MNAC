using System;
using Tests.States;

namespace Tests.Characters.Humanoid.Locomotion.Animations
{
    [Obsolete]
    internal class InAirMovementState : MovementState
    {
        public InAirMovementState(string name, float duration, MovementAnimator animator, bool enabled = true) : base(name == null ? "inair" : $"inair_{name}", duration, animator, enabled)
        {
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            animator.InAir = true;

        }
    }
}
