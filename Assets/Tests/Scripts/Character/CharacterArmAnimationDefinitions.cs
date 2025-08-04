using System;
using UnityEngine;

namespace Tests.Character
{
    [Serializable]
    public class CharacterArmAnimationDefinitions : ICharacterArmAnimationDefinitions
    {
        [SerializeField]
        AvatarMask _mask;

        public AvatarMask Mask { get => _mask; set => _mask = value; }
    }
}
