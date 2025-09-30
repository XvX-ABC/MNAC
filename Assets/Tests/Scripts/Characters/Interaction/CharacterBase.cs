using System;
using UnityEngine;

namespace Tests.Characters.Interaction
{
    public class CharacterBase : MonoBehaviour, ICharacter
    {
        Guid _id;
        public CharacterBase()
        {
            _id = Guid.NewGuid();
        }

        public Guid ID => _id;

        public string Name => this.name;
    }
}
