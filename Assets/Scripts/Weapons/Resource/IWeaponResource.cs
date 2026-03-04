using System;
using MNAC.Utilities.Assets;

namespace MNAC.Weapons
{
    public interface IWeaponLoader : IResourceLoader<Weapon>
    {
        public string Name { get; }
    }
}
