using System;
using Tests.Characters.Legs;
using Tests.States;

namespace Tests.Characters.Locomotion.Animations
{
    internal class GroundedMovementState : MovementState
    {
        LegsCore _legs;
        public GroundedMovementState(string name, float duration, LegsCore legs, MovementAnimator animator, bool enabled = true) : base(name == null ? "grounded" : $"grounded_{name}", duration, animator, enabled)
        {
            _legs = legs ?? throw new ArgumentNullException(nameof(legs));
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            animator.InAir = false;
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            _legs.Weight = currentTransition.Timeline.NormalizedTime;
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            _legs.Weight = 1 - currentTransition.Timeline.NormalizedTime;
        }
    }
}
