using System;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Sword.Animations
{
    public interface IArmedSwordArmAnimationDefinitions
    {
        public enum Transition
        {
            None,
            Idle_Boosting,
            Boosting_Idle,
            Slash_Idle,
        }
        public RuntimeAnimatorController WholeBodyController { get; }
        public RuntimeAnimatorController ArmController { get; }
        string VelocityName_Y { get; }
        string VelocityName_X { get; }
        string MirrorSwitch { get; }
        ActionDefinitions Boost { get; }
        ActionDefinitions Slash { get; }
        public StateTransitionOptions GetTransitionOptions(Transition transition);

    }
}
