using System;
using MNAC.States;

namespace MNAC.Characters
{
    internal class CharacterBehavioursStatemachine : WithCallbackPlayableStatemachine<object>
    {
        public CharacterBehavioursStatemachine(CharacterBehavioursStateContext context, string name, bool enabled = true) : base(name, enabled)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}
