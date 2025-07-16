using Assets.Scripts.Utilities.Timeline;

namespace Tests.States
{
    public interface IPlayableTransition<T> : ITransition<T>, IReadonlyPlayableTransition<T>
    {
        public ITimeline Timeline
        {
            get;
        }

    }
    public interface IReadonlyPlayableTransition<T>
    {
        public IReadonlyTimeline Timeline { get; }
    }
}
