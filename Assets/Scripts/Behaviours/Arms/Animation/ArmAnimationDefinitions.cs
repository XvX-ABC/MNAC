using System;
using Tests.Behaviours.Arms.Weapons.Animations;
using UnityEngine;

namespace Tests.Behaviours.Arms.Animations
{
    [Serializable]
    internal class ArmAnimationDefinitions : IArmAnimationDefinitions
    {
        [SerializeField]
        ArmWeaponAnimationDefinitions _weapon;

        public IArmWeaponAnimationDefinitions Weapon => _weapon;
    }
}
