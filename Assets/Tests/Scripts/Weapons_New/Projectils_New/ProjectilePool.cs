using Codice.CM.Common.Tree.Partial;
using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Tests.Weapons_New.Projectiles
{
    [Serializable]
    internal class ProjectilePool<T> : IProjectilePool<T> where T : class, IProjectile
    {
        [SerializeField]
        GameObject _prototype;
        [SerializeField]
        Transform _parent;
        ObjectPool<T> _pool;

        public Transform Parent { get => _parent; set => _parent = value; }
        private ProjectilePool()
        {
            _pool = new(CreateAmmo, GetAmmo, ReleaseAmmo, DestroyAmmo);
        }
        public ProjectilePool(GameObject prototype, ushort defaultCapacity = 10, ushort maxSize = 10000) : this(prototype, null, defaultCapacity, maxSize)
        {

        }
        public ProjectilePool(GameObject prototype, Transform parent, ushort defaultCapacity = 10, ushort maxSize = 10000)
        {
            if (!prototype.TryGetComponent<IProjectile>(out _))
                throw new ComponentCantFindException(prototype, typeof(T));
            _prototype = prototype ?? throw new ArgumentNullException(nameof(prototype));
            _parent = parent;
            _pool = new(CreateAmmo, GetAmmo, ReleaseAmmo, DestroyAmmo, false, defaultCapacity, maxSize);
        }
        T Instantiate()
        {
            var obj = default(GameObject);
            if (_parent == null)
                obj = UnityEngine.Object.Instantiate(_prototype);
            else
                obj = UnityEngine.Object.Instantiate(_prototype, _parent);
            return obj.GetComponent<T>();
        }
        T CreateAmmo()
        {
            var p = _prototype;
            var comp = Instantiate();
            var obj = comp.Obj;
            obj.name = p.name + "_" + _pool.CountAll;
            obj.SetActive(false);
            comp.ActionEndCallback += WhenProjectileActionEnd;
            return comp;
        }
        void GetAmmo(T comp)
        {
            var obj = comp.Obj;
            obj.SetActive(true);
            WeaponsHelper.PutInParent(obj, null);
        }
        void ReleaseAmmo(T comp)
        {
            var obj = comp.Obj;
            obj.SetActive(false);
            WeaponsHelper.PutInParent(obj, _parent);
        }
        void DestroyAmmo(T comp)
        {
            comp.ActionEndCallback -= WhenProjectileActionEnd;
        }
        void WhenProjectileActionEnd(IProjectile projectile)
        {
            var comp = projectile as T;
            Release(comp);
        }
        public T Get()
        {
            var comp = _pool.Get();
            return comp;
        }
        public void Release(T comp)
        {
            if (comp == null)
                throw new ArgumentNullException(nameof(comp));
            _pool.Release(comp);
        }
    }
}
