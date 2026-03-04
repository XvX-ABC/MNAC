using System;
using MNAC.Weapons.Launcher;

namespace Assets.Tests.Scripts.AI.BTExtensions
{
    internal abstract class LauncherActionBase : WeaponActionBase
    {
        protected ILauncher launcher;
        public override void OnStart()
        {
            base.OnStart();
            launcher = controller.currentWeapon as ILauncher ?? throw new NullReferenceException(nameof(launcher));
        }
    }
}
