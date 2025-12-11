using System;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons
{
    [Serializable]
    public class ArmWeaponDefinitions : Behaviours.Arms.Weapons.ArmWeaponDefinitions, IArmedWeaponArmDefinitions
    {
        [SerializeField]
        ArmedWeaponArmBehaviourBase_SO[] _weaponBehaviours;
        public IArmedWeaponArmBehaviour[] ArmedWeaponBehaviours => _weaponBehaviours;
    }
}
