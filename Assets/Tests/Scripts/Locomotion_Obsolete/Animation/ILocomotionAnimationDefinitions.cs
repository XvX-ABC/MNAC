namespace Tests.Locomotion.Animation
{
    public interface ILocomotionAnimationDefinitions
    {
        public IGroundLocomotionAnimatorDefinitions Ground { get; }
        public IJumpLocomotionAnimationDefinitions Jump { get; }
        public IAirLocomotionAnimationDefinitions Air { get; }
        public IBoostingLocomotionAnimatorDefinitions Boosting { get; }
    }
}