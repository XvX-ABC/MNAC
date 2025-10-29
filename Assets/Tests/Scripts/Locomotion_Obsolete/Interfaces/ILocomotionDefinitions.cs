using Locomotion;

namespace Tests.Locomotion_Obsolete
{
    public interface ILocomotionDefinitions
    {
        IBaseDefinitions Base { get; }
        IJumpDefinitions Jump { get; }
        IBoostingDefinitions Boosting { get; }
    }
}
