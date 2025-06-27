using System;

namespace Tests.States
{
    internal class Transition<T>
    {
        public IState<T> SourceState;
        public IState<T> DestinationState;
        public Func<bool> TriggerEvent;
    }
}
