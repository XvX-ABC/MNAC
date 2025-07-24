using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.Blackboards
{
    public class FieldChangeHandler<K, A> : IMiddleware<K, A>
    {
        K _key;
        protected bool _enabled;
        protected IReadOnlyDictionary<K, object> _values;
        protected Dictionary<K, Action<FieldEventType, object, object>> _actions;
        public bool Enabled { get => _enabled; set => _enabled = value; }

        public IReadOnlyDictionary<K, object> Values { set => _values = value; }
        public FieldChangeHandler(K key)
        {
            _key = key;
            _actions = new();
        }
        public bool Contains(K key)
        {
            return _actions.ContainsKey(key);
        }
        public void RegisterAction(K key, Action<FieldEventType, object, object> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            _actions[key] += action;
        }
        public void RegisterAction<T>(K key, Action<FieldEventType, T, T> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            RegisterAction(key, (e, o, n) =>
            {
                if (o is T oldValue)
                {
                    action(e, oldValue, (T)n);
                }
                else
                {
                    Debug.LogWarning($"The value type '{o.GetType()}' is not the expected type  '{typeof(T)}'.");
                }
            });
        }
        public void UnregisterAction<T>(K key, Action<FieldEventType, T, T> action)
        {
            if (!_actions.ContainsKey(key))
                return;
            var a = _actions[key];
            if (a == null)
                _actions.Remove(key);
            else
                _actions[key] -= action as Action<FieldEventType, object, object>;
        }
        public void Initialize(Blackboard<K, A> blackboard)
        {
            blackboard.TryRegisterField(_key, this);
        }
        (bool, object) IMiddleware<K, A>.ValueReadingHandle(ValueInfo<K, A> valueInfo)
        {
            var key = valueInfo.Key;
            if (_values.TryGetValue(key, out var oldValue) && _actions.TryGetValue(key, out var action))
            {
                action?.Invoke(FieldEventType.Reading, oldValue, valueInfo.Value);
            }
            return (true, valueInfo.Value);
        }

        (bool, object) IMiddleware<K, A>.ValueRegisterHandle(ValueInfo<K, A> valueInfo)
        {
            var key = valueInfo.Key;
            if (_values.TryGetValue(key, out var oldValue) && _actions.TryGetValue(key, out var action))
            {
                action?.Invoke(FieldEventType.Register, oldValue, valueInfo.Value);
            }
            return (true, valueInfo.Value);
        }

        (bool, object) IMiddleware<K, A>.ValueUnregisterHandle(ValueInfo<K, A> valueInfo)
        {
            var key = valueInfo.Key;
            if (_actions.TryGetValue(key, out var action))
            {
                action?.Invoke(FieldEventType.Unregister, valueInfo.Value, default);
            }
            return (true, valueInfo.Value);
        }

        (bool, object) IMiddleware<K, A>.ValueWritingHandle(ValueInfo<K, A> valueInfo)
        {
            var key = valueInfo.Key;
            if (_values.TryGetValue(key, out var oldValue) && _actions.TryGetValue(key, out var action))
            {
                action?.Invoke(FieldEventType.Writing, oldValue, valueInfo.Value);
            }
            return (true, valueInfo.Value);
        }


    }
}
