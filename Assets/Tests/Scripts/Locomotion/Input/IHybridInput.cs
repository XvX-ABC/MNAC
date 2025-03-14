namespace Tests.Locomotion
{
    public interface IHybridInput : IVirtualInput
    {
        public enum Mode
        {
            Player,
            Virtual,
        }
        public IHybridInput.Mode CurrentMode { get; set; }
    }
}