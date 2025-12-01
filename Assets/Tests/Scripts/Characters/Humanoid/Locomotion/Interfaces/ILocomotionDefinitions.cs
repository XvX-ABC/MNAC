using Tests.TPhysics.Locomotion;

namespace Tests.Characters.Humanoid.Locomotion
{
    public interface ILocomotionDefinitions
    {
        public IWalkingDefinitions Walking { get; }
        public IBoostingDefinitions Boosting { get; }
        public IJumpDefinitions Jump { get; }
        public IQuickBoostingDefinitions QuickBoosting { get; }
        public ushort PostureEvaluationFramesAmount { get; }
        public Animations.ILocomotionAnimatorDefinitions Animation { get; }
    }
}
