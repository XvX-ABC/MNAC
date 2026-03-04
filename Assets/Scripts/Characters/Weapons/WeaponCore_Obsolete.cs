using System;
using MNAC.Characters.Humanoid;
using MNAC.Utilities.Blackboards;
using MNAC.Weapons;
using UnityEngine;

namespace MNAC.Characters.Weapons
{
    [Obsolete]
    internal class WeaponCore_Obsolete : HumanoidComponent
    {
        public static explicit operator Weapons.WeaponCore_Obsolete(WeaponCore_Obsolete weaponCore)
        {
            return weaponCore._core;
        }
        [SerializeField]
        MNAC.Weapons.WeaponCore_Obsolete _core;
        protected override void Awake()
        {
            base.Awake();
            //_core = new(GetComponentsInChildren<IWeaponResource>());
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Weapon_Core, _core);
        }
        public override void Dispose()
        {
            blackboard.TryUnregisterField(CharacterBlackboardFields.Character_Weapon_Core);
            base.Dispose();
        }
    }
}
