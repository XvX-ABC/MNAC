using System;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons
{
    [Serializable]
    public class ArmWeaponDefinitions : Behaviours.Arms.Weapons.ArmWeaponDefinitions, IArmedWeaponArmDefinitions
    {
        [SerializeField]
        ArmedWeaponArmBehaviourBase_MonoComponent_Obsolete[] _armedBehaviours;
        public ArmedWeaponArmBehaviourBase_MonoComponent_Obsolete[] ArmedBehaviours => _armedBehaviours;
    }
}
