using System;
using Tests.Characters.Humanoid;
using Tests.Utilities.Blackboards;
using Tests.Weapons_New;
using UnityEngine;

namespace Tests.Characters.Weapons
{
    [Obsolete]
    internal class WeaponCore_Obsolete : HumanoidComponent
    {
        public static explicit operator Weapons_New.WeaponCore_Obsolete(WeaponCore_Obsolete weaponCore)
        {
            return weaponCore._core;
        }
        [SerializeField]
        Weapons_New.WeaponCore_Obsolete _core;
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
