namespace Tests.Weapons_New
{
    public interface IWeaponBackpack
    {
        IWeapon GetWeapon(string weaponName);
        void PutWeapon(string name, IWeapon weapon);
    }
}