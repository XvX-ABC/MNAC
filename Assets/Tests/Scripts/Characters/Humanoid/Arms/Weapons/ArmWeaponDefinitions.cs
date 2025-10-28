using System;
using Tests.Characters.Arms.Weapons.Launchers;
using UnityEngine;

namespace Tests.Characters.Arms.Weapons
{
    [Serializable]
    public class ArmWeaponDefinitions : Behaviours.Arms.Weapons.ArmWeaponDefinitions, IArmWeaponDefinitions
    {
        [SerializeField]
        ArmedWeaponArmBehaviourBase_MonoComponent_Obsolete[] _armedBehaviours;
        public ArmedWeaponArmBehaviourBase_MonoComponent_Obsolete[] ArmedBehaviours => _armedBehaviours;
    }
}
