using System;
using System.Collections.Generic;
using UnityEngine;

namespace MNAC.Utilities.Blackboards
{
    public class FieldChangeHandler<K, A> : IMiddleware<K, A>
    {
        internal class GeneratedDelegatesManager
        {
            internal Dictionary<int, Delegate> mapping;
            public GeneratedDelegatesManager()
            {
                mapping = new();
            }
        }
        K _key;
        protected bool _enabled;
        protected IReadOnlyDictionary<K, object> _values;
        protected Dictionary<K, Action<FieldEventType, object, object>> _mappingAction;
        protected Action<FieldEventType, object, object> _globalAction;
        GeneratedDelegatesManager _gdm;
        public bool Enabled { get => _enabled; set => _enabled = value; }
        public FieldChangeHandler(K key)
        {
            _key = key;
            _mappingAction = new();
            _gdm = new();
        }
        public bool Contains(K key)
        {
            return _mappingAction.ContainsKey(key);
        }
        public void RegisterGlobalAction(Action<FieldEventType, object, object> action)
        {
            _globalAction += action;
        }

        public void UnregisterGlobalAction(Action<FieldEventType, object, object> action)
        {
            _globalAction -= action;
        }
        public void RegisterAction(K key, Action<FieldEventType, object, object> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (_mappingAction.TryGetValue(key, out var a))
            {
                a += action;
                _mappingAction[key] = a;
            }
            else
            {
                _mappingAction.Add(key, action);
            }
        }
        public void RegisterAction<T>(K key, Action<FieldEventType, T, T> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            Action<FieldEventType, object, object> generatedAction = (e, o, n) =>
            {
                if ((o != null && o is not T) || (n != null && n is not T))
                {
                    Debug.LogWarning($"The values type '{o?.GetType()?.ToString() ?? "null"}, {n?.GetType()?.ToString() ?? "null"}' has one is not the expected type  '{typeof(T)}'.");
                    return;
                }
                action(e, (T)o, (T)n);
            };
            RegisterAction(key, generatedAction);
            _gdm.mapping.Add(action.GetHashCode(), generatedAction);
        }
        public void UnregisterAction<T>(K key, Action<FieldEventType, T, T> action)
        {
            if (_mappingAction.TryGetValue(key, out var dele))
            {

                var hashCode = action.GetHashCode();
                if (!_gdm.mapping.TryGetValue(hashCode, out var d))
                    return;
                dele -= (Action<FieldEventType, object, object>)d;
                if (dele == null)
                    _mappingAction.Remove(key);
                else
                    _mappingAction[key] = dele;

                _gdm.mapping.Remove(hashCode);
            }
            else
                return;
            //if (!_mappingAction.ContainsKey(key))
            //    return;
            //var a = _mappingAction[key];
            //if (a == null)
            //    _mappingAction.Remove(key);
            //else
            //{
            //    var dd = _mappingAction[key];
            //    var list = dd.GetInvocationList();
            //    Debug.Log("invocation list amount: " + list.Count());

            //    if (!_gdm.mapping.TryGetValue(action.GetHashCode(), out var d))
            //        throw new Exception();
            //    _mappingAction[key] -= (Action<FieldEventType, object, object>)d;
            //}
        }
        [Obsolete]
        public void RegisterAction_Obsolete<T>(K key, Action<FieldEventType, T, T> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            RegisterAction(key, (e, o, n) =>
            {
                if ((o != null && o is not T) || (n != null && n is not T))
                {
                    Debug.LogWarning($"The values type '{o?.GetType()?.ToString() ?? "null"}, {n?.GetType()?.ToString() ?? "null"}' has one is not the expected type  '{typeof(T)}'.");
                    return;
                }
                action(e, (T)o, (T)n);
                //if (o == null)
                //{
                //    action(e, default, (T)n);
                //}
                //else if (o is T oldValue)
                //{
                //    action(e, oldValue, (T)n);
                //}
                //else
                //{
                //}
            });
        }
        [Obsolete]
        public void UnregisterAction_Obsolete<T>(K key, Action<FieldEventType, T, T> action)
        {
            if (!_mappingAction.ContainsKey(key))
                return;
            var a = _mappingAction[key];
            if (a == null)
                _mappingAction.Remove(key);
            else
                _mappingAction[key] -= action as Action<FieldEventType, object, object>;
        }
        public void Initialize(Blackboard<K, A> blackboard)
        {
            blackboard.TryRegisterField(_key, this);
            _values = blackboard.values;
        }
        (bool, object) IMiddleware<K, A>.ValueReadingHandle(ValueInfo<K, A> valueInfo)
        {
            var key = valueInfo.Key;
            var oldValue = _values[key];
            if (_mappingAction.TryGetValue(key, out var action))
            {
                action?.Invoke(FieldEventType.Reading, oldValue, valueInfo.Value);
            }
            _globalAction?.Invoke(FieldEventType.Reading, oldValue, valueInfo.Value);
            return (true, valueInfo.Value);
        }

        (bool, object) IMiddleware<K, A>.ValueRegisterHandle(ValueInfo<K, A> valueInfo)
        {
            var key = valueInfo.Key;
            _values.TryGetValue(key, out var oldValue);
            if (_mappingAction.TryGetValue(key, out var action))
            {
                action?.Invoke(FieldEventType.Register, oldValue, valueInfo.Value);
            }
            _globalAction?.Invoke(FieldEventType.Register, oldValue, valueInfo.Value);
            return (true, valueInfo.Value);
        }

        (bool, object) IMiddleware<K, A>.ValueUnregisterHandle(ValueInfo<K, A> valueInfo)
        {
            var key = valueInfo.Key;
            var value = _values[key];
            if (_mappingAction.TryGetValue(key, out var action))
            {
                action?.Invoke(FieldEventType.Unregister, value, default);
            }
            _globalAction?.Invoke(FieldEventType.Unregister, value, default);
            return (true, valueInfo.Value);
        }

        (bool, object) IMiddleware<K, A>.ValueWritingHandle(ValueInfo<K, A> valueInfo)
        {
            var key = valueInfo.Key;
            var oldValue = _values[key];
            if (_mappingAction.TryGetValue(key, out var action))
            {
                action?.Invoke(FieldEventType.Writing, oldValue, valueInfo.Value);
            }
            _globalAction?.Invoke(FieldEventType.Writing, oldValue, valueInfo.Value);
            return (true, valueInfo.Value);
        }


    }
    public class FieldChangeHandler : FieldChangeHandler<object, object>
    {
        public FieldChangeHandler(object key) : base(key)
        {
        }
    }
}
