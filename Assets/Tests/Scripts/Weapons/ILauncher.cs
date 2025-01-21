using System;
using TMPro.EditorUtilities;
using Unity.VisualScripting;

namespace Tests.Weapons
{
    public interface ILauncher : IWeapon
    {
        public enum ActionsEnum
        {
            None = 0,
            Launch = 1,
            StartReload = 2,
            EndReload = 4,
            Supply = 8,
            All = 255,
        }
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