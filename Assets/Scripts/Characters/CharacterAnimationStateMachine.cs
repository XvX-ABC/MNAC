using MNAC.States;

namespace MNAC.Characters
{
    internal class CharacterAnimationStateMachine : WithCallbackPlayableStatemachine<object>
    {
        public CharacterAnimationStateMachine(string name, float duration = 0, bool enabled = true) : base(name, enabled)
        {
        }
    }
}
