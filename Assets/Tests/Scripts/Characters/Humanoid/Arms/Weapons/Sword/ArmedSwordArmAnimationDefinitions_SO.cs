using Tests.Behaviours.Arms.Weapons.Sword;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Sword
{
    [CreateAssetMenu(fileName = "ArmedSwordArmAnimationDefinitions", menuName = "Tests/Definitions/Characters/Humanoid/Arms/Weapons/Sword/ArmedSwordArmAnimationDefinitions")]
    public class ArmedSwordArmAnimationDefinitions_SO : ScriptableObject, IArmedSwordArmAnimationDefinitions
    {
        [SerializeField]
        private Behaviours.Arms.Weapons.Sword.Animations.ArmedSwordArmAnimationDefinitions _definitions;

        public RuntimeAnimatorController WholeBodyController => _definitions.WholeBodyController;

        public RuntimeAnimatorController ArmController => _definitions.ArmController;

        public string VelocityName_Y => _definitions.VelocityName_Y;

        public string VelocityName_X => _definitions.VelocityName_X;

        public string BoostingSwitchName => _definitions.BoostingSwitchName;

        public string BoostingSpeedMultiplierName => _definitions.BoostingSpeedMultiplierName;

        public float BoostingClipLength => _definitions.BoostingClipLength;

        public string SlashSwitchName => _definitions.SlashSwitchName;

        public string SlashSpeedMultiplierName => _definitions.SlashSpeedMultiplierName;

        public float SlashClipLength => _definitions.SlashClipLength;

        public StateTransitionOptions GetTransitionOptions(IArmedSwordArmAnimationDefinitions.Transition transition)
        {
            return _definitions.GetTransitionOptions(transition);
        }
    }


}
