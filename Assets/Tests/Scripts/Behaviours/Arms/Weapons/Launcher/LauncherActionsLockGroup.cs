using System;
using Unity.Collections.LowLevel.Unsafe;
using Utilities;
using ActionsEnum = Tests.Weapons.Launcher.ILauncher.ActionsEnum;
namespace Tests.Behaviours.Arms.Weapons.Launchers
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

        public void LockStartLaunch()
        {
            throw new NotImplementedException();
        }

        public void LockStartReload()
        {
            throw new NotImplementedException();
        }

        public bool EndLaunchLocked()
        {
            throw new NotImplementedException();
        }
    }
}