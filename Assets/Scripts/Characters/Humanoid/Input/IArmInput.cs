using Tests.Characters.Interaction.Input;

namespace Tests.Characters.Humanoid.Input
{
    public interface IArmInput
    {
        public bool WeaponSwitch { get; }
        IWeaponControlInput WeaponControl { get; }
    }
}
