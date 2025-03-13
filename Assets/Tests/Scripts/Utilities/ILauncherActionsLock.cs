using Utilities;
using static Tests.Weapons.Launcher.ILauncher;

namespace Tests.Utilities
{
    public interface ILauncherActionsLock : IActionsLock<ActionsEnum>
    {
        void LockStartLaunch();
        void LockStartReload();
        bool StartReloadLocked();
        bool EndReloadLocked();
        bool StartLaunchLocked();
        bool EndLaunchLocked();
        bool SupplyLocked();
    }
}