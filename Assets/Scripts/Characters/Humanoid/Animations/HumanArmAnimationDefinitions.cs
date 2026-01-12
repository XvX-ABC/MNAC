using System;
using Tests.Characters.Humanoid.Animations;
using UnityEngine;

namespace Tests.Characters.Animations
{
    [Serializable]
    public class HumanArmAnimationDefinitions : IHumanArmAnimationDefinitions
    {
        [SerializeField]
        AvatarMask _mask;

        public AvatarMask Mask { get => _mask; set => _mask = value; }
    }
}
