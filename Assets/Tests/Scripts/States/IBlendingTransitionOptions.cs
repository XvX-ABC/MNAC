namespace Tests.States
{
    public interface IBlendingTransitionOptions
    {
        float Duration { get; set; }
        float FixedExitTime { get; set; }
        InterruptionSource InterruptionSource { get; set; }
        float Offset { get; set; }
    }
}