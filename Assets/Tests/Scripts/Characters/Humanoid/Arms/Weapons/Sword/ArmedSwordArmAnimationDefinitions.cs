using Tests.Behaviours.Arms.Weapons.Launcher.Animations;
using Tests.Behaviours.Arms.Weapons.Sword;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Sword
{
    public class ArmedSwordArmAnimationDefinitions : MonoBehaviour, IArmedSwordArmAnimationDefinitions
    {
        [SerializeField]
        Behaviours.Arms.Weapons.Sword.Animations.ArmedSwordArmAnimationDefinitions _definitions;

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
            throw new System.NotImplementedException();
        }
    }


}
