using Tests.Characters.Animations;
using Tests.Characters.Humanoid.Animations;
using UnityEngine;

namespace Tests.Characters.C_0
{
    internal class C_0AnimationDefinitions_MonoComponent : MonoBehaviour, ICharacterAnimationDefinitions_C_0
    {
        [SerializeField]
        C_0AnimationDefinitions _definitons;

        public IStunningAnimationDefinitions Stunning => ((ICharacterAnimationDefinitions_C_0)_definitons).Stunning;

        public IDeathAnimationDefinitions Death => ((ICharacterAnimationDefinitions_C_0)_definitons).Death;

        public IHumanAnimationDefinitions HumanoidDefinitions => ((ICharacterAnimationDefinitions_C_0)_definitons).HumanoidDefinitions;
    }
}
