using System;
using Unity.Collections.LowLevel.Unsafe;
using Utilities;
using ActionsEnum = Tests.Weapons.ILauncher.ActionsEnum;
namespace Tests.Weapons
{
    public class LauncherActionsLockGroup : ActionsLockGroup<ActionsEnum>, ILauncherActionsLock
    {
        public LauncherActionsLockGroup(params ILauncherActionsLock[] subLocks) : base(subLocks)
        {

        }
        public bool EndReloadIsLocked()
        {
            return IsLocked(ActionsEnum.Launch);

        }

        public bool LaunchIsLocked()
        {
            return IsLocked(ActionsEnum.Launch);
        }

        public bool StartReloadIsLocked()
        {
            return IsLocked(ActionsEnum.StartReload);
        }
    }
}