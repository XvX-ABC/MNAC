using Tests.States;

namespace Tests.Behaviours.Arms.Weapons.Launchers.Animations
{
    internal class ArmedLauncherAnimationState : WithCallbackStatemachineState<object>
    {
        ArmedLauncherArmAnimator _animator;
        public ArmedLauncherAnimationState(ArmedLauncherArmAnimator animator, float duration = 0, bool enabled = true) : base(animator.statemachine, "armed_launcher_animation", duration, enabled)
        {
            _animator = animator;
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            statemachine.ChangeStateTo(_animator.AimingTarget == null ? _animator.idle : _animator.aiming);
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
            base.UpdateAction?.Invoke();
        }
    }
}
