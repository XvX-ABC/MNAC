using Tests.Behaviours.Arms.Weapons.Sword;
using Tests.Behaviours.Arms.Weapons.Sword.Animations;
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

        public ActionDefinitions Boost => _definitions.Boost;

        public ActionDefinitions Slash => _definitions.Slash;

        public string MirrorSwitch => ((IArmedSwordArmAnimationDefinitions)_definitions).MirrorSwitch;

        public StateTransitionOptions GetTransitionOptions(IArmedSwordArmAnimationDefinitions.Transition transition)
        {
            return _definitions.GetTransitionOptions(transition);
        }
    }


}
