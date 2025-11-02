using System;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons
{
    [Serializable]
    public class ArmWeaponDefinitions : Behaviours.Arms.Weapons.ArmWeaponDefinitions, IArmedWeaponArmDefinitions
    {
        [SerializeField]
        ArmedWeaponArmBehaviourBase_SO[] _armedBehaviours;
        public IArmedWeaponArmBehaviour[] ArmedWeaponBehaviours => _armedBehaviours;
    }
}
