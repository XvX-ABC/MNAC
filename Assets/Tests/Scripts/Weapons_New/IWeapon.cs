using System;

namespace Tests.Weapons_New
{
    public interface IWeapon
    {
        Guid ID { get; }
        string Name { get; }
        WeaponType Type { get; }
    }
}
