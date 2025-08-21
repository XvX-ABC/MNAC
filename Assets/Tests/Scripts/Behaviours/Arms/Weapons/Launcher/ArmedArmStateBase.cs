using Tests.States;

namespace Tests.BodyBehaviour.Arm.Weapons.Launcher
{
    internal abstract class ArmedArmStateBase : PlayableStateBase
    {
        protected ArmedArmStateBase(string name, float duration) : base($"armed_arm_{name}", duration)
        {
        }
        public override void OnEnter()
        {
        }
        public override void OnExit()
        {
        }
        public override void OnUpdate()
        {
        }
    }
}
