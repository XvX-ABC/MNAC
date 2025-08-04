using UnityEngine;

namespace Tests.Character
{
    public class CharacterAnimationDefinitions : MonoBehaviour, ICharacterAnimationDefinitions
    {
        [SerializeField]
        CharacterArmAnimationDefinitions _leftArm;
        [SerializeField]
        CharacterArmAnimationDefinitions _rightArm;

        public ICharacterArmAnimationDefinitions LeftArmDefinitions => _leftArm;

        public ICharacterArmAnimationDefinitions RightArmDefinitions => _rightArm;
    }
}
