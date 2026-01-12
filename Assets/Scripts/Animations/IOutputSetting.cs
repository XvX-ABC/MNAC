namespace Tests.Animations
{
    public interface IOutputSetting
    {
        public int PortNum { get; set; }
        public IAnimationPlayablePart Parent { get; set; }
        public float Weight { get; set; }
    }
}

