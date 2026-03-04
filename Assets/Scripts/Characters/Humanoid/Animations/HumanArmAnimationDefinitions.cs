using System;
using MNAC.Characters.Humanoid.Animations;
using UnityEngine;

namespace MNAC.Characters.Animations
{
    [Serializable]
    public class HumanArmAnimationDefinitions : IHumanArmAnimationDefinitions
    {
        [SerializeField]
        AvatarMask _mask;

        public AvatarMask Mask { get => _mask; set => _mask = value; }
    }
}
