using Assets.Scripts.Utilities.Timeline;
using System;
using System.Diagnostics;

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
    public interface IReadonlyPlayableTransition<T>
    {
        public IReadonlyTimeline Timeline { get; }
    }
}
