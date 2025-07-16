using System;

namespace Tests.States
{
    public interface ITransition<T>
    {
        public IState<T> SourceState { get; }
        public IState<T> DestinationState { get; }
        public Func<bool> TriggerEvent { get; }
        public Action<T> TriggeredEvent { get; set; }
    }
}
