using System;
using Tests.Weapons.Launcher;

namespace Tests.Weapons.MissileLauncher
{
    public interface IMissileLauncher : ILauncher
    {
        public ITarget_Obsolete Target { get; set; }
        public Action<IMissileLauncher, ITarget_Obsolete> TargetChangeAction { get; set; }
        public new IMissileLauncherDefinitions Definitions { get; }
    }
}