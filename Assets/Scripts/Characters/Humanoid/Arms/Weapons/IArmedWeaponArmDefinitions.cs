using System;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons
{
    public interface IArmedWeaponArmDefinitions : Behaviours.Arms.Weapons.IArmedWeaponArmDefinitions
    {
        IArmedWeaponArmBehaviour[] ArmedWeaponBehaviours { get; }
        [Obsolete]
        public IArmedWeaponArmBehaviour[] GetArmBehaviours(Transform parent);
    }
}
