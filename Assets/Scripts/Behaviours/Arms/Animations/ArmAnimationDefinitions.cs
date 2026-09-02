using System;
using MNAC.Behaviours.Arms.Weapons.Animations;
using UnityEngine;

namespace MNAC.Behaviours.Arms.Animations
{
    [Serializable]
    internal class ArmAnimationDefinitions : IArmAnimationDefinitions
    {
        [SerializeField]
        ArmWeaponAnimationDefinitions _weapon;

        public IArmWeaponAnimationDefinitions Weapon => _weapon;
    }
}
