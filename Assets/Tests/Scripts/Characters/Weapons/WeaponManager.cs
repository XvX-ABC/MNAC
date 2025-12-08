using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Utilities.Blackboards;
using Tests.Weapons_New;
using UnityEngine;

namespace Tests.Characters.Weapons
{
    internal abstract class WeaponManager : ScriptableObject, IWeaponManager
    {
        internal abstract IWeaponLoader[] loaders { get; }
        internal Weapons_New.WeaponManager weaponManager;

        public void Initialize()
        {
            weaponManager = new(loaders);
        }
        public void Dispose()
        {
            weaponManager.Dispose();
        }
        public bool Contains(string weaponName)
        {
            return weaponManager.Contains(weaponName);
        }

        public IWeapon GetWeapon(string weaponName)
        {
            return weaponManager.GetWeapon(weaponName);
        }

        public bool TryGetWeapon(string weaponName, out IWeapon weapon)
        {
            return weaponManager.TryGetWeapon(weaponName, out weapon);
        }
    }
}
