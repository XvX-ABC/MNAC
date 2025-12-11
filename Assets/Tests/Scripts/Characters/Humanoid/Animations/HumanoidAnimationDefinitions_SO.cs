using Tests.Characters.Animations;
using UnityEngine;

namespace Tests.Characters.Humanoid.Animations
{
    [CreateAssetMenu(fileName = "HumanoidAnimationDefinitions", menuName = SOHelper.DEFINITIONS_MENU_NAME + "/Animation/HumanoidAnimationDefinitions")]
    public class HumanoidAnimationDefinitions_SO : ScriptableObject, IHumanAnimationDefinitions
    {
        [SerializeField]
        HumanoidAnimationDefinitions _definitions;

        public IHumanArmAnimationDefinitions LeftArmDefinitions => _definitions.LeftArmDefinitions;

        public IHumanArmAnimationDefinitions RightArmDefinitions => _definitions.RightArmDefinitions;

        public IStunningAnimationDefinitions Stunning => _definitions.Stunning;

        public IDeathAnimationDefinitions Death => _definitions.Death;
    }
}
