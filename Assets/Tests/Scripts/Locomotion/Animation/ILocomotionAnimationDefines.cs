namespace Tests.Locomotion.Animation
{
    public interface ILocomotionAnimationDefines
    {
        public IHorizontalLocomotionAnimationDefines Horizontal { get; }
        public IJumpLocomotionAnimationDefines Jump { get; }
        public IAirLocomotionAnimationDefines Air { get; }
    }
}