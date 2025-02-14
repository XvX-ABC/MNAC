using Assets.Scripts.Utilities.Timeline;
using Assets.Tests.Scripts.Weapons;
using System;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using Utilities;

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


        public ITimeline ReloadTimeline { get; }
        public ILauncherDefines Defines { get; }
        public ushort SpareCount { get; }
        public ushort MagazineCount { get; }
        internal ILauncherActionsLock actionsLock { get; }
        public int Supply(int num);
        public bool StartReload();
        public bool EndReload();
        public bool Launch();
    }
    public interface IMissileLauncher : ILauncher
    {
        public ITarget Target { get; set; }
        public Action<IMissileLauncher,ITarget> TargetChangedAction { get; set; }
        public new IMissileLauncherDefines Defines { get; }
        public ITimeline DelayLaunchTimeline { get; }
    }
}