namespace MNAC.Weapons
{
    public interface IWeaponBackpack
    {
        IWeapon GetWeapon(string weaponName);
        void PutWeapon(string name, IWeapon weapon);
    }
}