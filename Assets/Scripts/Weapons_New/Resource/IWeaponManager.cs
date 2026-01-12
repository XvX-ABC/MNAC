using Tests.Weapons_New.Launcher;

namespace Tests.Weapons_New
{
    public interface IWeaponManager
    {
        bool Contains(string weaponName);
        void Dispose();
        IWeapon GetWeapon(string weaponName, bool reload = false);
        bool TryGetWeapon(string weaponName, out IWeapon weapon, bool reload = false);
    }
}