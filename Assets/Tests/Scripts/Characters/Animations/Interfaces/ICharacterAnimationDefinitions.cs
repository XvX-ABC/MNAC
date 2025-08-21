using UnityEngine;

namespace Tests.Characters.Animations
{
    [SerializeField]
    public interface ICharacterAnimationDefinitions
    {
        public ICharacterArmAnimationDefinitions LeftArmDefinitions { get; }
        public ICharacterArmAnimationDefinitions RightArmDefinitions { get; }
    }
}
