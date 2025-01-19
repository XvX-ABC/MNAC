using System;
using TMPro.EditorUtilities;
using Unity.VisualScripting;

namespace Tests.Weapons
{
    public interface ILauncher : IWeapon
    {
        public Action<ILauncher> InitializationAction { get; set; }
        public ILauncherDefines Defines { get; }
        public ushort SpareCount { get; }
        public ushort MagazineCount { get; }
        public int Supply(int num);
        public bool StartReload();
        public bool EndReload();
        public void Launch();
    }
}