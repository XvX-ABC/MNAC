using Tests.Behaviours.Arms.Weapons.Launcher.Animations;
using Tests.Behaviours.Arms.Weapons.Sword.Animations;
using UnityEngine;
using StateTransitionOptions = Tests.Behaviours.Arms.Weapons.Sword.StateTransitionOptions;

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

        public ActionDefinitions Boost => _definitions.Boost;

        public ActionDefinitions Slash => _definitions.Slash;

        public StateTransitionOptions GetTransitionOptions(IArmedSwordArmAnimationDefinitions.Transition transition)
        {
            throw new System.NotImplementedException();
        }
    }


}
