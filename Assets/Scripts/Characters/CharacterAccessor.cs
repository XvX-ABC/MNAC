using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNAC.Characters.Interaction;
using UnityEngine;

namespace MNAC.Characters
{
    internal abstract class CharacterAccessor : MonoBehaviour
    {
        public abstract ICharacter Character { get; set; }
    }
    internal abstract class CharacterAccessor<T> : CharacterAccessor where T : CharacterBase
    {
        internal T character;
        public override ICharacter Character
        {
            get => character;
            set
            {
                character = (T)value;
            }
        }
        public static implicit operator T(CharacterAccessor<T> accessor)
        {
            return accessor.character;
        }
    }
}
