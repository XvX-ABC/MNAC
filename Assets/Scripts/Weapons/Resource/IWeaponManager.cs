using MNAC.Weapons.Launcher;

namespace MNAC.Weapons
{
    public interface IWeaponManager
    {
        bool Contains(string weaponName);
        void Dispose();
        IWeapon GetWeapon(string weaponName, bool reload = false);
        bool TryGetWeapon(string weaponName, out IWeapon weapon, bool reload = false);
    }
}