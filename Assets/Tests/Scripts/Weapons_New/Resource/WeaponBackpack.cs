using Codice.CM.Common.Tree.Partial;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Graphs;

namespace Tests.Weapons_New
{
    public class WeaponBackpack : IWeaponBackpack
    {
        protected Dictionary<string, IWeapon> weapons;
        public WeaponBackpack()
        {
            weapons = new();
        }
        public virtual IWeapon GetWeapon(string weaponName)
        {
            if (weaponName == null)
                throw new ArgumentNullException(nameof(weaponName));
            if (weapons.TryGetValue(weaponName, out var weapon))
                return weapon;
            return null;
        }
        public virtual void PutWeapon(string name, IWeapon weapon)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (weapon == null)
                throw new ArgumentNullException(nameof(weapon));
            if (weapons.ContainsKey(name))
                weapons[name] = weapon;
            else
                weapons.Add(name, weapon);

        }
    }
}
