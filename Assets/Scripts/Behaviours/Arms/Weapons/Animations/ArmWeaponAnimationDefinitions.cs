using System;
using MNAC.Behaviours.Arms.Animations;
using UnityEngine;

namespace MNAC.Behaviours.Arms.Weapons.Animations
{
    [Serializable]
    public class ArmWeaponAnimationDefinitions : IArmWeaponAnimationDefinitions
    {
        [SerializeField]
        ArmSwitchingAnimationDefinitions _switching;

        public IArmWeaponSwitchingAnimationDefinitions Switching => _switching;
    }
}
