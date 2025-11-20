using Tests.Weapons.Projectiles_New;
using Tests.Weapons_New.Projectiles;
using UnityEngine;

namespace Tests.Weapons_New.Launcher
{
    internal class MachineGun : Launcher
    {
        [SerializeField]
        BulletPool_MonoComponent _bulletPool;

        protected override IProjectile GetProjectile()
        {
            return _bulletPool.Get();
        }

        protected override void ReleaseProjectile(IProjectile projectile)
        {
            _bulletPool.Release(projectile as Bullet);
        }
    }
}
