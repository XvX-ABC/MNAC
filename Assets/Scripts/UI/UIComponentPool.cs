using System;
using UnityEngine;
using UnityEngine.Pool;

namespace MNAC.UI
{
    public class UIComponentPool<T> where T : UIComponent
    {
        ObjectPool<T> _pool;
        Transform _parent;
        T _prototype;
        public UIComponentPool(T prototype, Transform parent = null, int defaultCapacity = 10, int maxSize = 10000)
        {
            _prototype = prototype ?? throw new ArgumentNullException(nameof(prototype));
            _pool = new(CreateInstance, GetInstance, ReleaseInstance, DestroyInstance, false, defaultCapacity, maxSize);
            _parent = parent;
        }
        public T Get()
        {
            return _pool.Get();
        }
        public void Release(T instance)
        {
            _pool.Release(instance);
        }
        public void Clear()
        {
            _pool.Clear();
        }
        protected virtual T CreateInstance()
        {
            var c = default(T);
            if (_parent == null)
                c = GameObject.Instantiate<T>(_prototype);
            else
                c = GameObject.Instantiate<T>(_prototype, _parent);
            var obj = c.gameObject;
            obj.SetActive(false);
            obj.name = _prototype.name + "_" + _pool.CountAll;
            return c;
        }
        protected virtual void GetInstance(T instance)
        {
            var obj = instance.gameObject;
            obj.SetActive(true);
        }
        protected virtual void ReleaseInstance(T instance)
        {
            var obj = instance.gameObject;
            obj.SetActive(false);
        }
        protected virtual void DestroyInstance(T instance)
        {
            instance.Dispose();
        }

    }
}
