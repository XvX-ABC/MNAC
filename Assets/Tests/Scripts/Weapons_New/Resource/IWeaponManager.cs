namespace Tests.Weapons_New
{
    public interface IWeaponManager
    {
        bool Contains(string weaponName);
        void Dispose();
        IWeapon GetWeapon(string weaponName);
        bool TryGetWeapon(string weaponName, out IWeapon weapon);
    }
}