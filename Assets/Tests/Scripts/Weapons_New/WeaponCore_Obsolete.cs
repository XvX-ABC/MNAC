using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.Weapons_New
{
    [Serializable]
    [Obsolete]
    public class WeaponCore_Obsolete : IDisposable
    {
        protected IWeaponLoader[] sources;
#if UNITY_EDITOR
        [SerializeField]
        List<string> _weaponNames;
#endif
        protected WeaponCore_Obsolete()
        {

        }
        public WeaponCore_Obsolete(IWeaponLoader[] sources)
        {
            this.sources = sources ?? throw new ArgumentNullException(nameof(sources));
            foreach (var s in this.sources)
                s.Load();
#if UNITY_EDIOR
            _weaponNames = new();
            foreach (var s in _sources)
                _weaponNames.Add(s.Name); 
#endif
        }
        ~WeaponCore_Obsolete()
        {
            Dispose();
        }
        int FindIndex(string weaponName)
        {
            if (weaponName.Length == 0)
                return -1;
            for (int i = 0; i < sources.Length; i++)
            {
                var s = sources[i];
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
            return sources[idx].Resource;
        }
        public bool TryGetWeapon(string weaponName, out IWeapon weapon)
        {
            weapon = GetWeapon(weaponName);
            return weapon != null;
        }

        public void Dispose()
        {
            foreach (var s in sources)
                s.Dispose();
        }

    }
}
