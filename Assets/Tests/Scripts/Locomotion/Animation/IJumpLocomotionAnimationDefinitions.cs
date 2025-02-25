namespace Tests.Locomotion.Animation
{
    public interface IJumpLocomotionAnimationDefinitions
    {
        public float AscendingClipLength { get; }
        public string AscendingMultiplierName { get; }
        public float LandingClipLength { get; }
        public string LandingMultiplierName { get; }
        //public string LandingValueParamName { get; }
        public string DescendingClipName { get; }
        public float DescendingClipLength { get; }
        public string EnterParamName { get; }
    }
}