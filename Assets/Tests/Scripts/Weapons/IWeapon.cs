using Tests.BodyBehaviour.Arm;

namespace Tests.Weapons
{
    public interface IWeapon
    {
        public string Name { get; }
        public WeaponType Type { get; }
    }
}