using Tests.Behaviours;

namespace Tests.Weapons
{
    public interface IWeapon : ILoad
    {
        public string Name { get; }
        public WeaponType Type { get; }
    }
}