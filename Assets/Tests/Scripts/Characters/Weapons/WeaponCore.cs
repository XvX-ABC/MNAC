using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Characters.Humanoid;
using Tests.Utilities.Blackboards;
using Tests.Weapons_New;
using UnityEngine;

namespace Tests.Characters.Weapons
{
    internal class WeaponCore : HumanoidComponent
    {
        public static explicit operator Weapons_New.WeaponCore(WeaponCore weaponCore)
        {
            return weaponCore._core;
        }
        [SerializeField]
        Weapons_New.WeaponCore _core;
        protected override void Awake()
        {
            base.Awake();
            _core = new(GetComponentsInChildren<IWeaponSource>());
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Weapon_Core, _core);
        }
        public override void Dispose()
        {
            base.Dispose();
            blackboard.TryUnregisterField(CharacterBlackboardFields.Character_Weapon_Core);
        }
    }
}
