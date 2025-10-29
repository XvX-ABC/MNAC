namespace Tests.Characters.Interaction.Input
{
    public interface IArmInput
    {
        public bool WeaponSwitch { get; }
        IWeaponControlInput WeaponControl { get; }
    }
}
