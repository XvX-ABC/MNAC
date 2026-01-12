using Tests.Characters.Animations;
using Tests.Characters.Humanoid.Animations;
using UnityEngine;

namespace Tests.Characters.C_0
{
    internal class C_0AnimationDefinitionsLoader : MonoBehaviour, ICharacterAnimationDefinitions_C_0
    {
        [SerializeField]
        C_0AnimationDefinitions_SO _definitions;

        public IHumanAnimationDefinitions HumanoidDefinitions => _definitions.HumanoidDefinitions;

        public IStunningAnimationDefinitions Stunning => _definitions.Stunning;

        public IDeathAnimationDefinitions Death => _definitions.Death;
    }
}
