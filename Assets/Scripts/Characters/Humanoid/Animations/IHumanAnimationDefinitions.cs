using MNAC.Characters.Animations;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Animations
{
    [SerializeField]
    public interface IHumanAnimationDefinitions
    {
        public IHumanArmAnimationDefinitions LeftArmDefinitions { get; }
        public IHumanArmAnimationDefinitions RightArmDefinitions { get; }
    }
}
