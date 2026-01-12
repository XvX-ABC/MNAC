using Tests.States;

namespace Tests.Behaviours.Arm.Weapons
{
    internal abstract class ArmedArmStateBase : WithCallbackPlayableState
    {
        protected ArmedArmStateBase(string name, float duration) : base($"armed_arm_{name}", duration)
        {
        }
        protected ArmedArmStateBase(string name, float duration, bool enabled) : base($"armed_arm_{name}", duration, enabled)
        {
        }
        //public override void OnEnter()
        //{
        //}
        //public override void OnExit()
        //{
        //}
        //public override void OnUpdate()
        //{
        //}
    }
}
