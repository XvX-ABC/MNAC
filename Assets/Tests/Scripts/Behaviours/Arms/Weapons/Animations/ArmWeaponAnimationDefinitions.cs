using System;
using Tests.Behaviours.Arms.Animations;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Animations
{
    [Serializable]
    public class ArmWeaponAnimationDefinitions : IArmWeaponAnimationDefinitions
    {
        [SerializeField]
        ArmSwitchingAnimationDefinitions _switching;

        public IArmWeaponSwitchingAnimationDefinitions Switching => _switching;
    }
}
