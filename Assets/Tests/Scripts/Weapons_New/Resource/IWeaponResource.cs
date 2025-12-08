using System;
using Tests.Utilities.Assets_New;

namespace Tests.Weapons_New
{
    public interface IWeaponLoader : IResourceLoader<Weapon>
    {
        public string Name { get; }
    }
}
