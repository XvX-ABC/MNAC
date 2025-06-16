namespace Tests.Locomotion.Animation
{
    public interface IAirLocomotionAnimationDefinitions
    {
        public string EnterParamName { get; }
        public string XParamName { get; }
        public string YParamName { get; }


        public string DescentClipName { get; }
        public string NextStateClipName { get; }
        public float V0 { get; }
    }
}