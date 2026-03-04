using System;
using MNAC.Behaviours.Arms.Animations;
using MNAC.Characters.Humanoid.Arms.Weapons;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Arms
{
    [Serializable]
    internal class ArmDefinitions : IArmDefinitions
    {
        [SerializeField]
        ArmWeaponDefinitions _weapon;
        [SerializeField]
        ArmAnimationDefinitions _animation;
        public IArmedArmDefinitions Weapon => _weapon;

        public IArmAnimationDefinitions Animation => _animation;
    }
}
