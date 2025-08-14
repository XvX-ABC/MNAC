using Tests.States;

internal interface IBlendingTransition<T> : IPlayableTransition<T>
{
    public float FixedExitTime { get; }
}

