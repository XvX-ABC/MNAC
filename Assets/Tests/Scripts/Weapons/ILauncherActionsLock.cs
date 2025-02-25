using Utilities;
using static Tests.Weapons.ILauncher;

namespace Tests.Weapons
{
    public interface ILauncherActionsLock:IActionsLock<ActionsEnum>
    {
        bool StartReloadLocked();
        bool EndReloadLocked();
        bool LaunchLocked();
        bool SupplyLocked();
    }
}