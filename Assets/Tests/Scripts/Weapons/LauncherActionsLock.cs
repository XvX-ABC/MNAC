using Utilities;
using ActionsEnum = Tests.Weapons.ILauncher.ActionsEnum;
namespace Tests.Weapons
{
    public class LauncherActionsLock : ActionsLock<ActionsEnum>, ILauncherActionsLock
    {
        public bool LaunchLocked()
        {
            return IsLocked(ActionsEnum.Launch);
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