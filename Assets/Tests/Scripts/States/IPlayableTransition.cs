using Assets.Scripts.Utilities.Timeline;

namespace Tests.States
{
    public interface IPlayableTransition<T> : ITransition<T>
    {
        public ITimeline Timeline { get; }
    }
}
