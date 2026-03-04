using System;
using MNAC.Characters.Animations;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Animations
{
    [Obsolete]
    [CreateAssetMenu(fileName = "HumanoidAnimationDefinitions", menuName = SOHelper.DEFINITIONS_MENU_NAME + "/Animation/HumanoidAnimationDefinitions")]
    public class HumanoidAnimationDefinitions_SO : ScriptableObject, IHumanAnimationDefinitions
    {
        [SerializeField]
        HumanoidAnimationDefinitions _definitions;

        public IHumanArmAnimationDefinitions LeftArmDefinitions => _definitions.LeftArmDefinitions;

        public IHumanArmAnimationDefinitions RightArmDefinitions => _definitions.RightArmDefinitions;

    }
}
