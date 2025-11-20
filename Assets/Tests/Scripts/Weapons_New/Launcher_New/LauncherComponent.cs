using System;

namespace Tests.Weapons_New.Launcher
{
    internal class LauncherComponent : WeaponComponent
    {
        static LauncherComponent()
        {
            OwnerLauncher = Guid.NewGuid();
        }
        public static readonly Guid OwnerLauncher;
    }
}
