using System;
using System.Linq;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Sword.Animations
{
    [Serializable]
    public class ArmedSwordArmAnimationDefinitions : IArmedSwordArmAnimationDefinitions
    {
        [SerializeField]
        RuntimeAnimatorController _wholeBodyController;
        [SerializeField]
        RuntimeAnimatorController _armController;
        [SerializeField]
        string _velocityName_Y;
        [SerializeField]
        string _velocityName_X;
        [SerializeField]
        string _mirrorSwitch;
        [SerializeField]
        ActionDefinitions _boost;
        [SerializeField]
        ActionDefinitions _slash;
        [SerializeField]
        StateTransitionOptions[] _transitionOptions;
        public ArmedSwordArmAnimationDefinitions()
        {

        }


        public RuntimeAnimatorController WholeBodyController => _wholeBodyController;

        public RuntimeAnimatorController ArmController => _armController;

        public string VelocityName_Y => _velocityName_Y;

        public string VelocityName_X => _velocityName_X;


        public ActionDefinitions Boost => _boost;

        public ActionDefinitions Slash => _slash;

        public string MirrorSwitch { get => _mirrorSwitch; }

        public StateTransitionOptions GetTransitionOptions(IArmedSwordArmAnimationDefinitions.Transition transition)
        {
            return _transitionOptions.FirstOrDefault(t => t.Transition == transition);
        }
    }
}
