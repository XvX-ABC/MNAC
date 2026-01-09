using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tests.Utilities.Blackboards
{
    public struct ValueInfo<K, A>
    {
        public K Key;
        public object Value;
        public A AdditionalInfo;

        public ValueInfo(K key, object value, A additionalInfo)
        {
            Key = key;
            Value = value;
            AdditionalInfo = additionalInfo;
        }
    }
    public enum FieldEventType
    {
        Register,
        Unregister,
        Reading,
        Writing,
    }
    public class Blackboard<K, A> : ICloneable
    {
        protected internal Dictionary<K, object> values;
        IMiddleware<K, A>[] _middlewares;
        protected IMiddleware<K, A>[] middlewares
        {
            get => _middlewares;
            set
            {
                _middlewares = value;
                InitializeMiddlewares();
            }
        }
        delegate (bool, object) MiddlewareFunc(IMiddleware<K, A> m, ValueInfo<K, A> valueInfo);
        protected Blackboard(params IMiddleware<K, A>[] middlewares)
        {
            values = new Dictionary<K, object>();
            this.middlewares = middlewares;
        }
        public Blackboard()
        {
            values = new Dictionary<K, object>();
        }
        void InitializeMiddlewares()
        {
            if (_middlewares == null)
                return;
            foreach (var m in _middlewares)
            {
                m.Enabled = false;
                m.Initialize(this);
            }

            foreach (var m in _middlewares)
                m.Enabled = true;
        }
        (bool, object) MiddlewareExecute(ValueInfo<K, A> info, FieldEventType etype)
        {
            if (_middlewares == null || _middlewares.Length == 0)
                return (true, info.Value);
            MiddlewareFunc func = null;
            switch (etype)
            {
                case FieldEventType.Register:
                    func = Register;
                    break;
                case FieldEventType.Unregister:
                    func = Unregister;
                    break;
                case FieldEventType.Reading:
                    func = Read;
                    break;
                case FieldEventType.Writing:
                    func = Write;
                    break;
            }
            foreach (var m in _middlewares)
            {
                if (!m.Enabled)
                    continue;
                var (passed, d) = func(m, info);
                if (!passed)
                {
                    Debug.LogWarning($"The middleware '{m.GetType()}' intercepted this data operation '{etype}'");
                    return (false, null);
                }
                info.Value = d;
            }
            return (true, info.Value);
            (bool, object) Register(IMiddleware<K, A> m, ValueInfo<K, A> info) => m.ValueRegisterHandle(info);
            (bool, object) Unregister(IMiddleware<K, A> m, ValueInfo<K, A> info) => m.ValueUnregisterHandle(info);
            (bool, object) Read(IMiddleware<K, A> m, ValueInfo<K, A> info) => m.ValueReadingHandle(info);
            (bool, object) Write(IMiddleware<K, A> m, ValueInfo<K, A> info) => m.ValueWritingHandle(info);
        }
        public bool Contains(K key)
        {
            return values.ContainsKey(key);
        }
        public bool TryRegisterField(K key, object value = null, A additionalInfo = default)
        {
            var (passed, d) = MiddlewareExecute(new(key, value, additionalInfo), FieldEventType.Register);
            if (!passed)
                return false;
            if (values.TryGetValue(key, out var od))
            {
                Debug.LogWarning($"The value registering of this time will to replaces the value '{od}' to '{d}'.");
                values[key] = d;
            }
            else
                values.Add(key, d);
            return true;
        }
        public bool TryUnregisterField(K key, A additionalInfo = default)
        {
            if (!values.TryGetValue(key, out var value))
                return false;
            var (passed, d) = MiddlewareExecute(new(key, value, additionalInfo), FieldEventType.Unregister);
            if (!passed)
                return false;
            values.Remove(key);
            return true;
        }

        public bool TryReadValue(K key, out object value, A additionalInfo = default)
        {
            if (!values.TryGetValue(key, out value))
                return false;
            var (passed, d) = MiddlewareExecute(new(key, value, additionalInfo), FieldEventType.Reading);
            if (!passed)
                return false;
            value = d;
            return true;
        }
        public bool TryReadValue<T>(K key, out T value, A additionalInfo = default)
        {
            value = default;
            if (!TryReadValue(key, out var obj, additionalInfo))
                return false;
            if (obj is T result)
                value = result;
            else
                return false;
            return true;

        }
        public bool TryWriteValue<T>(K key, T value, A additionalInfo = default)
        {
            if (!values.ContainsKey(key))
                return false;
            var (passed, d) = MiddlewareExecute(new ValueInfo<K, A>(key, value, additionalInfo), FieldEventType.Writing);
            if (!passed)
                return false;
            values[key] = d;
            return true;
        }

        public object Clone()
        {
            var obj = new Blackboard<K, A>();
            obj._middlewares = (IMiddleware<K, A>[])_middlewares.Clone();
            obj.values = new Dictionary<K, object>(values);
            return obj;
        }
    }
    public class Blackboard : Blackboard<object, object>
    {
        protected FieldChangeHandler<object, object> handler
        {
            get => (FieldChangeHandler<object, object>)this.middlewares[0];
        }
        public Blackboard() : base(new FieldChangeHandler(MiddlewareFields.FieldChangeHandler))
        {
        }

        protected Blackboard(params IMiddleware<object, object>[] middlewares) : base(middlewares)
        {
            this.middlewares.Append(new FieldChangeHandler(MiddlewareFields.FieldChangeHandler));
        }
        public void TryReadValueOrThrowException<T>(object key, out T value)
        {
            if (!TryReadValue<T>(key, out value))
                throw new BlackboardKeyNotFoundException(key);
        }
        public void TryRegisterFieldOrWriteValue<T>(object key, T value)
        {
            if (!TryRegisterField(key, value))
                TryWriteValue(key, value);
        }

    }
    public class BlackboardException : Exception
    {
        public BlackboardException(string message) : base(message)
        {

        }
    }
    public class BlackboardKeyNotFoundException : BlackboardException
    {
        public BlackboardKeyNotFoundException(object key) : base($"Key '{key}' not found in blackboard.")
        {

        }
    }
}
