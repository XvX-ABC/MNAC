using System;
using Tests.Interaction;
using Tests.Weapons.Launcher;

namespace Tests.Weapons.MissileLauncher
{
    public interface IMissileLauncher : ILauncher
    {
        public IGameObjTarget Target { get; set; }
        public Action<IMissileLauncher, IGameObjTarget> TargetChangeAction { get; set; }
        public new IMissileLauncherDefinitions Definitions { get; }
    }
}