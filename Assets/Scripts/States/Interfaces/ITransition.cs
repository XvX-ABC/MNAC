using System;
using System.Collections.Generic;

namespace Tests.States
{
    public interface ITransition<T> : IReadonlyTransition<T>
    {
        public Action<T> TriggeredEvent { get; set; }

        public bool Contains(Func<bool> triggerEvent);
        public void AddTriggerEvent(Func<bool> triggerEvent);
        public void RemoveTriggerEvent(Func<bool> triggerEvent);
        public void ClearTriggerEvents();
    }
    public interface IReadonlyTransition<T>
    {
        public IState<T> SourceState { get; }
        public IState<T> DestinationState { get; }
        public bool Triggered { get; }
        public IReadOnlyList<Func<bool>> TriggerEvents { get; }
    }
}
