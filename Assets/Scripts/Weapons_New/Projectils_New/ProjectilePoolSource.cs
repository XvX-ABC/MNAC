using UnityEngine;

namespace Tests.Weapons_New.Projectiles
{
    internal abstract class ProjectilePoolSource<T> : MonoBehaviour, IProjectilePoolSource<T> where T : class, IProjectile
    {
        public abstract IProjectilePool<T> Pool { get; }

        public abstract void Dispose();
        public abstract void Initialize();
    }
}
