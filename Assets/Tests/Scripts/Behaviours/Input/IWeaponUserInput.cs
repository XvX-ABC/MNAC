namespace Tests.Characters.Interaction.Input
{
    public interface IWeaponUserInput
    {
        public bool Switch { get; }
        public IWeaponControlInput Control { get; }
    }
    
}
