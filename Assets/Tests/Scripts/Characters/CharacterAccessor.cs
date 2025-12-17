using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tests.Characters
{
    internal abstract class CharacterAccessor<T> : MonoBehaviour where T : CharacterBase
    {
        internal T character;
        public static implicit operator T(CharacterAccessor<T> accessor)
        {
            return accessor.character;
        }
    }
}
