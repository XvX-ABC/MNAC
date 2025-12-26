using System;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons
{
    public interface IArmedWeaponArmDefinitions : Behaviours.Arms.Weapons.IArmedWeaponArmDefinitions
    {
        [Obsolete]
        IArmedWeaponArmBehaviour[] ArmedWeaponBehaviours { get; }
        public IArmedWeaponArmBehaviour[] GetArmBehaviours(Transform parent);
    }
}
