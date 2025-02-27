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
        public bool StartReloadLocked()
        {
            return IsLocked(ActionsEnum.StartReload);
        }
        public bool EndReloadLocked()
        {
            return IsLocked(ActionsEnum.StartLaunch);

        }

        public bool StartLaunchLocked()
        {
            return IsLocked(ActionsEnum.StartLaunch);
        }
        public bool SupplyLocked()
        {
            return IsLocked(ActionsEnum.Supply);
        }

    }
}