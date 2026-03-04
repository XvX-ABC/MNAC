namespace MNAC.Input
{
    public interface IHybridInput : IVirtualInput
    {
        public enum Mode
        {
            Player,
            Virtual,
        }
        public Mode CurrentMode { get; set; }
    }
}