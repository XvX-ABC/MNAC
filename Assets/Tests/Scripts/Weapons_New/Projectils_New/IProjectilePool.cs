using UnityEngine;

namespace Tests.Weapons_New.Projectiles
{
    internal interface IProjectilePool<T> where T : class, IProjectile
    {
        Transform Parent { get; set; }

        T Get();
        void Release(T comp);
    }
}