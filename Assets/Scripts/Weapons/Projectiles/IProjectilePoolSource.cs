using System;

namespace MNAC.Weapons.Projectiles
{
    internal interface IProjectilePoolSource<T> : IDisposable where T : class, IProjectile
    {
        IProjectilePool<T> Pool { get; }
        public void Initialize();
    }
}
