using System;

namespace Tests.Weapons_New.Projectiles
{
    internal interface IProjectilePoolSource<T> : IDisposable where T : class, IProjectile
    {
        IProjectilePool<T> Pool { get; }
        public void Initialize();
    }
}
