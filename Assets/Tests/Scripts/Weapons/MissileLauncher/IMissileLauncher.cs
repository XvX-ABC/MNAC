using System;
using Tests.Weapons.Launcher;

namespace Tests.Weapons.MissileLauncher
{
    public interface IMissileLauncher : ILauncher
    {
        public ITarget Target { get; set; }
        public Action<IMissileLauncher, ITarget> TargetChangeAction { get; set; }
        public new IMissileLauncherDefinitions Definitions { get; }
    }
}