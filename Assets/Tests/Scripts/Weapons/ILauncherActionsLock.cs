using Utilities;
using static Tests.Weapons.ILauncher;

namespace Tests.Weapons
{
    public interface ILauncherActionsLock:IActionsLock<ActionsEnum>
    {
        bool EndReloadIsLocked();
        bool LaunchIsLocked();
        bool StartReloadIsLocked();
    }
}