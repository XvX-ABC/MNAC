using Tests.Weapons;

namespace Tests.Behaviours.Arm
{
    public interface IArmWeaponAction : IArmAction
    {
        public WeaponType Type { get; }
        public IWeapon Weapon { get; set; }

    }
}
