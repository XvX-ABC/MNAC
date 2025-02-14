using Utilities;
using ActionsEnum = Tests.Weapons.ILauncher.ActionsEnum;
namespace Tests.Weapons
{
    public class LauncherActionsLock : ActionsLock<ActionsEnum>, ILauncherActionsLock
    {
        public bool LaunchIsLocked()
        {
            return IsLocked(ActionsEnum.Launch);
        }
        public bool StartReloadIsLocked()
        {
            return IsLocked(ActionsEnum.StartReload);
        }
        public bool EndReloadIsLocked()
        {
            return IsLocked(ActionsEnum.EndReload);
        }
    }
}