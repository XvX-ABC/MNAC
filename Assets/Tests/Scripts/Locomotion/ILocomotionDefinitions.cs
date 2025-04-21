namespace Locomotion
{
    public interface ILocomotionDefinitions
    {
        IBaseDefinitions Base { get; }
        IJumpDefinitions Jump { get; }
        IBoostingDefinitions Boosting { get; }
    }
}
