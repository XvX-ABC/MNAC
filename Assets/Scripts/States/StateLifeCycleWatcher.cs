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
        LifeCycleState _currentLifeCycle;

        public LifeCycleState CurrentState { get => _currentLifeCycle; }

        public StateLifeCycleWatcher(WithCallbackPlayableState<T> boundState)
        {
            _boundState = boundState ?? throw new ArgumentNullException(nameof(boundState));
            _currentLifeCycle = LifeCycleState.Ready;

            _boundState.EntryAction = WhenStateEntered;
            _boundState.UpdateAction = WhenStateUpdate;
            _boundState.ExitAction = WhenStateExited;
        }
        void WhenStateEntered()
        {
            _currentLifeCycle = LifeCycleState.Entered;
        }
        void WhenStateUpdate()
        {
            _currentLifeCycle = LifeCycleState.Update;
        }
        void WhenStateExited()
        {
            _currentLifeCycle = LifeCycleState.Exited;
        }

    }
}
