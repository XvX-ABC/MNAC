using Assets.Scripts.Utilities.Timeline;

namespace Tests.States
{
    public interface ITransition<T>
    {
        public IState<T> SourceState { get; }
        public IState<T> DestinationState { get; }
    }
    public interface IPlayableTransition<T> : ITransition<T>
    {
        public ITimeline Timeline { get; }
    }
}
