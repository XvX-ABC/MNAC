using System;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Sword
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
        string BoostingSwitchName { get; }
        string BoostingSpeedMultiplierName { get; }
        float BoostingClipLength { get; }
        string SlashSwitchName { get; }
        string SlashSpeedMultiplierName { get; }
        float SlashClipLength { get; }
        public StateTransitionOptions GetTransitionOptions(Transition transition);

    }
}
