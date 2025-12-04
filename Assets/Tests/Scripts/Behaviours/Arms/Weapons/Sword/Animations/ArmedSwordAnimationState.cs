using Tests.States;

namespace Tests.Behaviours.Arms.Weapons.Sword.Animations
{
    internal class ArmedSwordAnimationState : WithCallbackStatemachineState<object>
    {
        ArmedSwordArmAnimator _animator;
        public ArmedSwordAnimationState(ArmedSwordArmAnimator animator, float duration = 0, bool enabled = true) : base(animator.statemachine, "armed_launcher_animation", duration, enabled)
        {
            _animator = animator;
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            statemachine.ChangeStateTo(_animator.idle);
            base.FromPreviousStateTransitionBegin(currentTransition);
        }
        public override void OnEnter()
        {
            statemachine.Enabled = true;
            base.OnEnter();
        }
        public override void OnExit()
        {
            statemachine.Enabled = false;
            base.OnExit();
        }
        public override void OnUpdate()
        {
            //base.UpdateAction?.Invoke();
            base.OnUpdate();
        }
    }
}
