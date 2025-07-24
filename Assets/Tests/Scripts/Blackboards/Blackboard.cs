using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tests.Blackboards
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
    public class Blackboard<K, A>
    {
        protected Dictionary<K, object> values;
        protected IMiddleware<K, A>[] middlewares;

        delegate (bool, object) MiddlewareFunc(IMiddleware<K, A> m, ValueInfo<K, A> valueInfo);
        protected Blackboard(params IMiddleware<K, A>[] middlewares) : this()
        {
            this.middlewares = middlewares;
        }
        public Blackboard()
        {
            values = new Dictionary<K, object>();
            InitializeMiddlewares();
        }
        void InitializeMiddlewares()
        {
            if (middlewares == null)
                return;
            foreach (var m in middlewares)
            {
                m.Enabled = false;
                m.Initialize(this);
            }

            foreach (var m in middlewares)
                m.Enabled = true;
        }
        (bool, object) MiddlewareExecute(ValueInfo<K, A> info, FieldEventType etype)
        {
            if (middlewares == null || middlewares.Length == 0)
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
            foreach (var m in middlewares)
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
            if (this.values.TryGetValue(key, out var od))
            {
                Debug.LogWarning($"The value registering of this time will to replaces the value '{od}' to '{d}'.");
                this.values[key] = od;
            }
            else
                this.values.Add(key, d);
            return true;
        }
        public bool TryUnregisterField(K key, A additionalInfo = default)
        {
            if (!this.values.TryGetValue(key, out var value))
                return false;
            var (passed, d) = MiddlewareExecute(new(key, value, additionalInfo), FieldEventType.Unregister);
            if (!passed)
                return false;
            this.values.Remove(key);
            return true;
        }

        public bool TryReadValue(K key, out object value, A additionalInfo = default)
        {
            if (!this.values.TryGetValue(key, out value))
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
            if (!this.TryReadValue(key, out var obj, additionalInfo))
                return false;
            if (obj is T result)
                value = result;
            else
                return false;
            return true;

        }
        public bool TryWriteValue<T>(K key, T value, A additionalInfo = default)
        {
            if (!this.values.ContainsKey(key))
                return false;
            var (passed, d) = MiddlewareExecute(new ValueInfo<K, A>(key, value, additionalInfo), FieldEventType.Writing);
            if (!passed)
                return false;
            this.values[key] = d;
            return true;
        }

    }
}
