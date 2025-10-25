using Tests.Utilities.MountPoints;

namespace Tests.Weapons
{
    public interface IWeapon : ILoad
    {
        public string Name { get; }
        public WeaponType Type { get; }
    }
}