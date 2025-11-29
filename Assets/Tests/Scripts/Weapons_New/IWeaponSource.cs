using System;

namespace Tests.Weapons_New
{
    public interface IWeaponSource : IDisposable
    {
        public string Name { get; }
        public IWeapon Weapon { get; }
        public void Initialize();
    }
}
