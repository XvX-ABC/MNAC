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
        public IHumanArmAnimationDefinitions LeftArmDefinitions => _leftArm;

        public IHumanArmAnimationDefinitions RightArmDefinitions => _rightArm;
    }
}
