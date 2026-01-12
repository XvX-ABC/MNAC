using UnityEngine;

namespace Tests.Weapons_New.Projectiles
{
    internal class ProjectilePool_MonoComponent<T> : MonoBehaviour, IProjectilePool<T> where T : class, IProjectile
    {
        [SerializeField]
        ProjectilePool<T> _projectilePool;

        public Transform Parent { get => _projectilePool.Parent; set => _projectilePool.Parent = value; }
        void Awake()
        {
            if (_projectilePool.Parent == null)
                _projectilePool.Parent = this.transform;
        }
        public T Get()
        {
            return _projectilePool.Get();
        }

        public void Release(T comp)
        {
            _projectilePool.Release(comp);
        }
    }
}
