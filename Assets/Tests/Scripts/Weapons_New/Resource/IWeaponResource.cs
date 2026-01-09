using System;
using Tests.Utilities.Assets;

namespace Tests.Weapons_New
{
    public interface IWeaponLoader : IResourceLoader<Weapon>
    {
        public string Name { get; }
    }
}
