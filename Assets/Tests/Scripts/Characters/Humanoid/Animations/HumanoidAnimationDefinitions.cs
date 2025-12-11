using System;
using Tests.Characters.Animations;
using UnityEngine;

namespace Tests.Characters.Humanoid.Animations
{
    [Serializable]
    public class HumanoidAnimationDefinitions : IHumanAnimationDefinitions
    {
        [SerializeField]
        HumanArmAnimationDefinitions _leftArm;
        [SerializeField]
        HumanArmAnimationDefinitions _rightArm;
        [SerializeField]
        StunningAnimationDefinitions _stunning;
        [SerializeField]
        DeathAnimationDefinitions _death;
        public IHumanArmAnimationDefinitions LeftArmDefinitions => _leftArm;

        public IHumanArmAnimationDefinitions RightArmDefinitions => _rightArm;

        public IStunningAnimationDefinitions Stunning => _stunning;

        public IDeathAnimationDefinitions Death => _death;
    }
}
