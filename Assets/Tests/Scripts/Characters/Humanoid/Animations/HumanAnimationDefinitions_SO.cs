using Tests.Characters.Animations;
using UnityEngine;

namespace Tests.Characters.Humanoid.Animations
{
    [CreateAssetMenu(fileName = "HumanAnimationDefinitions", menuName = SOHelper.DEFINITIONS_MENU_NAME + "/Animation/HumanAnimationDefinitions")]
    public class HumanAnimationDefinitions_SO : ScriptableObject, IHumanAnimationDefinitions
    {
        [SerializeField]
        HumanAnimationDefinitions _definitions;

        public IHumanArmAnimationDefinitions LeftArmDefinitions => _definitions.LeftArmDefinitions;

        public IHumanArmAnimationDefinitions RightArmDefinitions => _definitions.RightArmDefinitions;

        public IStunningAnimationDefinitions Stunning => _definitions.Stunning;

        public IDeathAnimationDefinitions Death => _definitions.Death;
    }
}
