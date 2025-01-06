using TMPro.EditorUtilities;
using Unity.VisualScripting;

namespace Tests.Weapons
{
    public interface ILauncher : IWeapon
    {
        public void Suppelement(ushort num);
        public bool StartReload();
        public bool EndReload();
        public void Reload();
        public void Fire();
    }
}