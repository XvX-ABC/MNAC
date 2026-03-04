using MNAC.States;

namespace MNAC.Characters
{
    internal class CharacterBehaviourStateBase : WithCallbackPlayableState<object>
    {
        protected new CharacterBehavioursStateContext context { get => (CharacterBehavioursStateContext)base.context; }
        public CharacterBehaviourStateBase(string name, float duration = 0, bool enabled = true) : base(name == null ? "character" : $"character_{name}", duration, enabled)
        {
        }
    }
}
