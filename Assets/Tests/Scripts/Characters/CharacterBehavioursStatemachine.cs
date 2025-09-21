using System;
using Tests.States;
using TMPro.EditorUtilities;
using UnityEngine;

namespace Tests.Characters
{
    internal class CharacterBehavioursStatemachine : WithCallbackPlayableStatemachine<object>
    {
        public CharacterBehavioursStatemachine(CharacterBehavioursStateContext context, string name, bool enabled = true) : base(name, enabled)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}
