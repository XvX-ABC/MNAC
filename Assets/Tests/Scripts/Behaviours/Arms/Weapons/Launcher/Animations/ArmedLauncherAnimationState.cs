using Tests.States;

namespace Tests.Behaviours.Arms.Weapons.Launchers.Animations
{
    internal class ArmedLauncherAnimationState : WithCallbackStatemachineState<object>
    {
        public ArmedLauncherAnimationState(WithCallbackPlayableStatemachine<object> statemachine, float duration = 0, bool enabled = true) : base(statemachine, "armed_launcher_animation", duration, enabled)
        {
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
