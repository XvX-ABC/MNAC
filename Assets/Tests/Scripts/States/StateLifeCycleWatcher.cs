using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.States
{
    public class StateLifeCycleWatcher<T>
    {
        WithCallbackPlayableState<T> _boundState;
        LifeCycle _currentLifeCycle;

        public LifeCycle CurrentLifeCycle { get => _currentLifeCycle; }

        public StateLifeCycleWatcher(WithCallbackPlayableState<T> boundState)
        {
            _boundState = boundState ?? throw new ArgumentNullException(nameof(boundState));
            _currentLifeCycle = LifeCycle.Ready;

            _boundState.EntryAction = WhenStateEntered;
            _boundState.UpdateAction = WhenStateUpdate;
            _boundState.ExitAction = WhenStateExited;
        }
        void WhenStateEntered()
        {
            _currentLifeCycle = LifeCycle.Entered;
        }
        void WhenStateUpdate()
        {
            _currentLifeCycle = LifeCycle.Update;
        }
        void WhenStateExited()
        {
            _currentLifeCycle = LifeCycle.Exited;
        }

    }
}
