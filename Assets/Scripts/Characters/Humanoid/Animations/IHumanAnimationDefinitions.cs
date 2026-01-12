using Tests.Characters.Animations;
using UnityEngine;

namespace Tests.Characters.Humanoid.Animations
{
    [SerializeField]
    public interface IHumanAnimationDefinitions
    {
        public IHumanArmAnimationDefinitions LeftArmDefinitions { get; }
        public IHumanArmAnimationDefinitions RightArmDefinitions { get; }
    }
}
