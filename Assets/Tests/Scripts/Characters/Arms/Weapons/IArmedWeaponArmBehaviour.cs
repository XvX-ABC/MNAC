using Tests.States;
using Tests.Utilities.Composable;

namespace Tests.Characters.Arms
{
    public interface IArmedWeaponArmBehaviour : Behaviours.Arms.IArmedWeaponArmBehaviour, IComponent, IWithCallbackPlayableState<object>
    {

    }
}
