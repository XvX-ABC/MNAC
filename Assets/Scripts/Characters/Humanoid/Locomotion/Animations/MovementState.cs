using Tests.States;

namespace Tests.Characters.Humanoid.Locomotion.Animations
{
    internal class MovementState : LocomotionAnimationStateBase
    {
        protected internal MovementAnimator animator;
        public MovementState(string name, float duration, MovementAnimator animator, bool enabled = true) : base(name == null ? "movement" : $"movement_{name}", duration, enabled)
        {
            this.animator = animator;
        }
        override public void OnEnter()
        {
            animator.Weight = 1;
        }
        public override void OnUpdate()
        {
            base.OnUpdate();

            animator.Update();
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            animator.Weight = currentTransition.Timeline.NormalizedTime;
            OnUpdate();
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            animator.Weight = 1 - currentTransition.Timeline.NormalizedTime;
            OnUpdate();
        }
        public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionEnd(currentTransition);
            animator.Weight = 0;
        }
    }
}
