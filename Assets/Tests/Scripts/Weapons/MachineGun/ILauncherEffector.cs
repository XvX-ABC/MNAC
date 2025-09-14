using System;
using Tests.Weapons.Launcher;

namespace Tests.Weapons.MachineGuns
{
    public interface ILauncherEffector : IDisposable
    {
        public ILauncher Owner { get; }
        public void Initialize(ILauncher owner);
    }
}
