using BehaviorDesigner.Runtime;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace Tests.Utilities
{
    [Serializable]
    public class MonoComponentPool<T> where T : Component
    {
        [SerializeField]
        GameObject _origin;
        [SerializeField]
        Transform _parent;
        ObjectPool<T> _pool;

        public Transform Parent { get => _parent; set => _parent = value; }
        protected MonoComponentPool()
        {
            _pool = new(CreateInstance, GetInstance, ReleaseInstance, DestroyInstance);
        }
        public MonoComponentPool(GameObject origin, Transform parent = null)
        {
            _origin = origin ?? throw new ArgumentNullException(nameof(origin));
            if (!_origin.TryGetComponent<T>(out _))
                throw new ComponentCantFindException(_origin, typeof(T));
            _parent = parent;
            _pool = new(CreateInstance, GetInstance, ReleaseInstance, DestroyInstance);
        }
        protected virtual T CreateInstance()
        {
            var obj = GameObject.Instantiate(_origin, _parent);
            obj.name = _origin.name + "_" + _pool.CountAll;
            var ca = obj.GetComponent<T>();
            obj.SetActive(false);
            return ca;
        }
        protected virtual void GetInstance(T instance)
        {
            instance.gameObject.SetActive(true);
        }
        protected virtual void ReleaseInstance(T instance)
        {
            instance.gameObject.SetActive(false);
        }
        protected virtual void DestroyInstance(T instance)
        {

        }
        public T Get()
        {
            return _pool.Get();
        }
        public void Release(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            _pool.Release(instance);
        }
    }
}
