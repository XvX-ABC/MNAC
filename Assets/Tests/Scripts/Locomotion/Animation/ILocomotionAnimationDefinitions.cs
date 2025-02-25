namespace Tests.Locomotion.Animation
{
    public interface ILocomotionAnimationDefinitions
    {
        public IHorizontalLocomotionAnimationDefinitions Horizontal { get; }
        public IJumpLocomotionAnimationDefinitions Jump { get; }
        public IAirLocomotionAnimationDefinitions Air { get; }
    }
}