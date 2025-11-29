using System;
using Tests.Utilities.MountPoints;

namespace Tests.Weapons
{
    [Obsolete]
    public interface IWeapon_Obsolete : ILoad
    {
        public string Name { get; }
        public WeaponType Type { get; }
    }
}