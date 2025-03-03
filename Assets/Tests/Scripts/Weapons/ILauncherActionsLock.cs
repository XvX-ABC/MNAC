using Utilities;
using static Tests.Weapons.ILauncher;

namespace Tests.Weapons
{
    public interface ILauncherActionsLock:IActionsLock<ActionsEnum>
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