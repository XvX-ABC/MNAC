using Locomotion;

namespace Tests.Characters.Locomotion
{
    public interface ILocomotionDefinitions
    {
        public IWalkingDefinitions Walking { get; }
        public IBoostingDefinitions Boosting { get; }
        public IJumpDefinitions Jump { get; }
        public IQuickBoostingDefinitions QuickBoosting { get; }
        public ushort PostureEvaluationFramesQuantity { get; }
    }
}
