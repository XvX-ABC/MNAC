using Tests.Characters.Animations;
using Tests.Characters.Humanoid.Animations;
using UnityEngine;

namespace Tests.Characters.C_0
{
    [CreateAssetMenu(fileName = "C_0_AnimationDefinitions", menuName = SOHelper.DEFINITIONS_MENU_NAME + "/C_0Animation")]
    internal class C_0AnimationDefinitions_SO : ScriptableObject, ICharacterAnimationDefinitions_C_0
    {
        [SerializeField]
        C_0AnimationDefinitions _definitions;

        public IHumanAnimationDefinitions HumanoidDefinitions => ((ICharacterAnimationDefinitions_C_0)_definitions).HumanoidDefinitions;

        public IStunningAnimationDefinitions Stunning => ((ICharacterAnimationDefinitions_C_0)_definitions).Stunning;

        public IDeathAnimationDefinitions Death => ((ICharacterAnimationDefinitions_C_0)_definitions).Death;
    }
}
