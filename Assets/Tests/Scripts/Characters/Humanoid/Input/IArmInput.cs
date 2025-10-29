using Tests.Characters.Interaction.Input;

namespace Tests.Characters.Humanoid.Interaction.Input
{
    public interface IArmInput
    {
        public bool WeaponSwitch { get; }
        IWeaponControlInput WeaponControl { get; }
    }
}
