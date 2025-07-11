using Assets.Scripts.Utilities.Timeline;
using System;

namespace Tests.States
{
    public class Transition<T> : ITransition<T>
    {
        internal StateBase<T> sourceState;
        internal StateBase<T> destinationState;
        internal Func<bool> triggerEvent;
        public IState<T> SourceState { get => sourceState; }
        public IState<T> DestinationState { get => destinationState; }

        public Func<bool> TriggerEvent => triggerEvent;

        public override int GetHashCode()
        {
            return HashCode.Combine(sourceState, destinationState, triggerEvent);
        }
    }
    internal class DurationTimeTransition<T> : Transition<T>
    {
        protected internal ITimeline timeline;
        public DurationTimeTransition()
        {

        }
    }
}
