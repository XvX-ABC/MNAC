using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utilities
{
    public class StateEvent<TState, TArg>
    {
        TState _oldState;
        Action<TState, TState, TArg> _action;
        public StateEvent(Action<TState, TState, TArg> action)
        {
            _action = action ?? throw new NullReferenceException(nameof(action));
        }
        bool TryExecuteWhenStateChanged(TState currentState, TArg arg)
        {
            if (_oldState.Equals(currentState))
                return false;
            _action(_oldState, currentState, arg);
            _oldState = currentState;
            return true;
        }
        public bool TryExecute(TState currentState, TArg arg)
        {
            return TryExecuteWhenStateChanged(currentState, arg);
        }
    }
    public struct SingleEvent
    {
        Action _action;
        bool _enabled;
        public bool Enabled { get => _enabled; set => _enabled = value; }
        public SingleEvent(Action action, bool defaultEnabled)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _enabled = defaultEnabled;
        }
        public SingleEvent(Action action) : this(action, false)
        {
        }
        public bool TryExecute()
        {
            if (!_enabled)
                return false;
            _action();
            _enabled = false;
            return true;
        }
    }
    public struct SingleEvent<TArg>
    {
        Action<TArg> _action;
        bool _enabled;
        public bool Enabled { get => _enabled; set => _enabled = value; }
        public SingleEvent(Action<TArg> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _enabled = false;
        }

        public bool TryExecute(TArg arg)
        {
            if (!_enabled)
                return false;
            _action(arg);
            _enabled = false;
            return true;
        }
    }
}
