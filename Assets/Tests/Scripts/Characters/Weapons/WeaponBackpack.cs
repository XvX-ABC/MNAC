using System;
using Tests.Utilities;
using Tests.Utilities.Assets.Tests.Scripts.Utilities.Extensions;
using Tests.Utilities.Blackboards;
using Tests.Weapons_New;
using UnityEngine;

namespace Tests.Characters.Weapons
{
    internal class WeaponBackpack : CharacterComponent, IWeaponBackpack
    {
        public static explicit operator Weapons_New.WeaponBackpack(WeaponBackpack backpack)
        {
            return backpack._weaponBackpack;
        }
        #region internal classes
        internal class InternalWeaponBackpack : Weapons_New.WeaponBackpack
        {
            WeaponManager _weaponManager;
            Transform _parent;
            public InternalWeaponBackpack(WeaponManager weaponManager, Transform parent)
            {
                _weaponManager = weaponManager ?? throw new ArgumentNullException(nameof(weaponManager));
                _parent = parent;

                //LoadWeaponsFromWeaponCore();
            }
            internal void LoadWeaponsFromWeaponCore()
            {
                foreach (var loader in _weaponManager.loaders)
                    PutWeapon(loader.Name, loader.Resource);
            }
            public override bool ContainsWeapon(string weaponName)
            {
                return base.ContainsWeapon(weaponName) || _weaponManager.Contains(weaponName);
            }
            public override IWeapon GetWeapon(string weaponName)
            {
                var w = base.GetWeapon(weaponName);
                if (w == null)
                {
                    w = _weaponManager.GetWeapon(weaponName, true);
                    weapons.Add(w.Name, w);
                }
                var obj = w.Obj;
                obj.PutInParent(null);
                return w;
            }

            public override void PutWeapon(string name, IWeapon weapon)
            {
                try
                {
                    base.PutWeapon(name, weapon);
                    weapon.Obj.transform.PutInParent(_parent);
                }
                catch (Exception)
                {
                    throw;
                }
            }

        }
        #endregion
        [SerializeField]
        WeaponManager _weaponManager;
        [SerializeField]
        Transform _weaponsContainer;
        InternalWeaponBackpack _weaponBackpack;

        protected override void Awake()
        {
            base.Awake();
            _weaponManager.Initialize();
            var parent = this.transform;
            if (_weaponsContainer != null)
                parent = _weaponsContainer;
            _weaponBackpack = new(_weaponManager, parent);
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Weapon_Core, _weaponManager);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Weapon_Backpack, this);
        }
        public override void Dispose()
        {
            blackboard.TryUnregisterField(CharacterBlackboardFields.Character_Weapon_Backpack);
            blackboard.TryUnregisterField(CharacterBlackboardFields.Character_Weapon_Core);
            base.Dispose();
        }
        public bool ContainsWeapon(string weaponName)
        {
            return _weaponBackpack.ContainsWeapon(weaponName);
        }
        public IWeapon GetWeapon(string weaponName)
        {
            return _weaponBackpack.GetWeapon(weaponName);
        }

        public void PutWeapon(string name, IWeapon weapon)
        {
            _weaponBackpack.PutWeapon(name, weapon);
        }
    }
}
