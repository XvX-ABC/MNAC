using Tests.Utilities;
using ActionsEnum = Tests.Weapons.Launcher.ILauncher.ActionsEnum;
namespace Tests.Behaviours.Arms.Weapons.Launcher
{
    public class LauncherActionsLock : ActionsLock<ActionsEnum>, ILauncherActionsLock
    {
        public void LockStartLaunch()
        {
            Lock(ActionsEnum.All ^ ActionsEnum.EndLaunch);
        }
        public void LockStartReload()
        {
            Lock(ActionsEnum.All ^ ActionsEnum.EndReload);
        }
        public bool StartLaunchLocked()
        {
            return IsLocked(ActionsEnum.StartLaunch);
        }
        public bool EndLaunchLocked()
        {
            return IsLocked(ActionsEnum.EndLaunch);
        }
        public bool SupplyLocked()
        {
            return IsLocked(ActionsEnum.Supply);
        }
        public bool StartReloadLocked()
        {
            return IsLocked(ActionsEnum.StartReload);
        }
        public bool EndReloadLocked()
        {
            return IsLocked(ActionsEnum.EndReload);
        }
    }
}