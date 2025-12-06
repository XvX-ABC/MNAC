using Tests.States;

namespace Tests.Characters
{
    internal class CharacterAnimationStateMachine : WithCallbackPlayableStatemachine<object>
    {
        public CharacterAnimationStateMachine(string name, float duration = 0, bool enabled = true) : base(name, enabled)
        {
        }
    }
}
