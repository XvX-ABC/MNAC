using Assets.Scripts.Utilities.Timeline;

namespace Tests.States
{
    public interface IPlayableTransition<T> : ITransition<T>, IReadonlyPlayableTransition<T>
    {
        public new ITimeline Timeline
        {
            get;
        }
        public byte InterruptionSource { get; }

    }
    public interface IReadonlyPlayableTransition<T>
    {
        public IReadonlyTimeline Timeline { get; }
    }
}
