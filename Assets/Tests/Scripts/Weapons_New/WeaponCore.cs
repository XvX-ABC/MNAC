using Codice.CM.Common.Tree.Partial;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Graphs;
using UnityEngine;

namespace Tests.Weapons_New
{
    [Serializable]
    public class WeaponCore : IDisposable
    {
        IWeaponSource[] _sources;
#if UNITY_EDITOR
        [SerializeField]
        List<string> _weaponNames;
#endif
        private WeaponCore()
        {

        }
        public WeaponCore(IWeaponSource[] sources)
        {
            _sources = sources ?? throw new ArgumentNullException(nameof(sources));
            foreach (var s in _sources)
                s.Initialize();
#if UNITY_EDIOR
            _weaponNames = new();
            foreach (var s in _sources)
                _weaponNames.Add(s.Name); 
#endif
        }
        ~WeaponCore()
        {
            Dispose();
        }
        int FindIndex(string weaponName)
        {
            if (weaponName.Length == 0)
                return -1;
            for (int i = 0; i < _sources.Length; i++)
            {
                var s = _sources[i];
                if (s == null)
                    continue;
                if (s.Name == weaponName)
                    return i;
            }
            return -1;
        }
        public bool Contains(string weaponName)
        {
            return FindIndex(weaponName) > -1;
        }
        public IWeapon GetWeapon(string weaponName)
        {
            var idx = FindIndex(weaponName);
            if (idx < 0)
                return null;
            return _sources[idx].Weapon;
        }
        public bool TryGetWeapon(string weaponName, out IWeapon weapon)
        {
            weapon = GetWeapon(weaponName);
            return weapon != null;
        }

        public void Dispose()
        {
            foreach (var s in _sources)
                s.Dispose();
        }

    }
}
