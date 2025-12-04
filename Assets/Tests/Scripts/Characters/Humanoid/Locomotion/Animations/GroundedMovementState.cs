using System;
using Tests.Characters.Humanoid.Legs;
using Tests.States;

namespace Tests.Characters.Humanoid.Locomotion.Animations
{
    internal class GroundedMovementState : MovementState
    {
        LegsController _legs;
        public GroundedMovementState(string name, float duration, LegsController legs, MovementAnimator animator, bool enabled = true) : base(name == null ? "grounded" : $"grounded_{name}", duration, animator, enabled)
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
