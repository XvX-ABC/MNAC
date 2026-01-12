using System;
using System.Diagnostics;
using Tests.Utilities.Timeline;

namespace Tests.States
{
    public interface IPlayableTransition<T> : ITransition<T>, IReadonlyPlayableTransition<T>
    {
        public new ITimeline Timeline
        {
            get;
        }
        public InterruptionSource InterruptionSource { get; }

    }
    public interface IReadonlyPlayableTransition<T> : IReadonlyTransition<T>
    {
        public IReadonlyTimeline Timeline { get; }
    }
}
