using Tests.States;

namespace Tests.Characters.Arms
{
    public interface IArmedWeaponArmBehaviour : Behaviours.Arms.IArmedWeaponArmBehaviour, ICharacterComponent, IWithCallbackPlayableState<object>
    {

    }
}
