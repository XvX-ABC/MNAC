using UnityEngine;

namespace Tests.Weapons_New.Projectiles
{
    internal class ProjectilePoolPrefabSource<T> : ProjectilePoolSource<T> where T : class, IProjectile
    {
        [SerializeField]
        ProjectilePool_MonoComponent<T> _prefab;
        ProjectilePool_MonoComponent<T> _instance;
        public override IProjectilePool<T> Pool => _instance;

        public override void Dispose()
        {
        }

        public override void Initialize()
        {
            _instance = Object.FindAnyObjectByType<ProjectilePool_MonoComponent<T>>();
            if (_instance != null)
                return;
            _instance = GameObject.Instantiate(_prefab);
            _instance.gameObject.SetActive(true);
        }
    }
}
