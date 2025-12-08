using System;

namespace Tests.Weapons_New
{
    public class WeaponManager : IDisposable, IWeaponManager
    {
        protected IWeaponLoader[] loaders;

        public WeaponManager(IWeaponLoader[] loaders)
        {
            this.loaders = loaders;
        }

        int FindIndex(string weaponName)
        {
            for (int i = 0; i < loaders.Length; i++)
            {
                var s = loaders[i];
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
            var l = loaders[idx];
            if (l.Resource == null)
                l.Load();
            return loaders[idx].Resource;
        }
        public bool TryGetWeapon(string weaponName, out IWeapon weapon)
        {
            weapon = GetWeapon(weaponName);
            return weapon != null;
        }
        public void Dispose()
        {
            foreach (var l in loaders)
                l.Dispose();
        }
    }

}
