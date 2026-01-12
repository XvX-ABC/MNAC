using System;
using Tests.Behaviours.Arms.Animations;
using Tests.Characters.Humanoid.Arms.Weapons;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms
{
    [Serializable]
    internal class ArmDefinitions : IArmDefinitions
    {
        [SerializeField]
        ArmWeaponDefinitions _weapon;
        [SerializeField]
        ArmAnimationDefinitions _animation;
        public IArmedWeaponArmDefinitions Weapon => _weapon;

        public IArmAnimationDefinitions Animation => _animation;
    }
}
