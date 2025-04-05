using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utilities
{
    public struct SingleEvent
    {
        Action _action;
        bool _enabled;
        public bool Enabled { get => _enabled; set => _enabled = value; }
        public SingleEvent(Action action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _enabled = false;
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
