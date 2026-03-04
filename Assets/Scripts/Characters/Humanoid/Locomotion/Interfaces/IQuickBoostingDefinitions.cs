namespace MNAC.Characters.Humanoid.Locomotion
{
    public interface IQuickBoostingDefinitions : IBoostingDefinitions
    {
        public float Duration { get; }
        public float ColdDownTime { get; }
    }
}
