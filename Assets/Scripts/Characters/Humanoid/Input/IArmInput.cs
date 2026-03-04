using MNAC.Characters.Interaction.Input;

namespace MNAC.Characters.Humanoid.Input
{
    public interface IArmInput
    {
        public bool WeaponSwitch { get; }
        IWeaponControlInput WeaponControl { get; }
    }
}
