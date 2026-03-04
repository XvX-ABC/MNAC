using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNAC.Utilities.Blackboards;
using MNAC.Weapons;
using UnityEngine;

namespace MNAC.Characters.Weapons
{
    internal abstract class WeaponManager : ScriptableObject, IWeaponManager
    {
        internal abstract IWeaponLoader[] loaders { get; }
        internal MNAC.Weapons.WeaponManager weaponManager;

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

        public IWeapon GetWeapon(string weaponName, bool reload = false)
        {
            return weaponManager.GetWeapon(weaponName, reload);
        }

        public bool TryGetWeapon(string weaponName, out IWeapon weapon, bool reload = false)
        {
            return weaponManager.TryGetWeapon(weaponName, out weapon, reload);
        }
    }
}
