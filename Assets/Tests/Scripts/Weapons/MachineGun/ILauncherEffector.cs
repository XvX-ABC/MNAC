using System;
using Tests.Weapons.Launcher;

namespace Tests.Weapons.MachineGuns
{
    public interface ILauncherEffector : IDisposable
    {
        public ILauncher_Obsolete Owner { get; }
        public void Initialize(ILauncher_Obsolete owner);
    }
}
