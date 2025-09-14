using UnityEngine;

namespace Tests.Characters.Animations
{
    public class CharacterAnimationDefinitions : MonoBehaviour, ICharacterAnimationDefinitions
    {
        [SerializeField]
        CharacterArmAnimationDefinitions _leftArm;
        [SerializeField]
        CharacterArmAnimationDefinitions _rightArm;
        [SerializeField]
        StunningAnimationDefinitions _stunning;
        public ICharacterArmAnimationDefinitions LeftArmDefinitions => _leftArm;

        public ICharacterArmAnimationDefinitions RightArmDefinitions => _rightArm;

        public IStunningAnimationDefinitions Stunning => _stunning;
    }
}
