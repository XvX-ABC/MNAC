using Assets.Tests.Scripts.Weapons;
using System;

namespace Tests.Weapons
{
    public interface IMissileLauncher : ILauncher
    {
        public ITarget Target { get; set; }
        public Action<IMissileLauncher, ITarget> TargetChangeAction { get; set; }
        public new IMissileLauncherDefinitions Definitions { get; }
    }
}