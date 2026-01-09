namespace Tests.Characters.Humanoid.Animations
{
    internal class HumanAnimationStatemachine : CharacterAnimationStateMachine
    {
        public HumanAnimationStatemachine(bool enabled = true) : base("humanoid_statemachine", 0, enabled)
        {
        }
    }
}
