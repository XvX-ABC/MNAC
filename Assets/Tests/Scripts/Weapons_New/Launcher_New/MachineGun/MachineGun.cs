using Tests.Weapons.Projectiles_New;
using Tests.Weapons_New.Projectiles;
using UnityEngine;

namespace Tests.Weapons_New.Launcher
{
    internal class MachineGun : Launcher
    {
        [SerializeField]
        ProjectilePoolSource<Bullet> _bulletPoolSource;
        IProjectilePool<Bullet> _bulletPool;
        protected override void Start()
        {
            base.Start();
            _bulletPoolSource.Initialize();
            _bulletPool = _bulletPoolSource.Pool;
        }
        protected internal override IProjectile Launch()
        {
            var bullet = (Bullet)base.Launch();
            var ray = new Ray(MuzzleTrans.position, this.transform.forward);
            bullet.ShootingRay = ray;
            return bullet;
        }
        protected override IProjectile GetProjectile()
        {
            return _bulletPool.Get();
        }

        protected override void ReleaseProjectile(IProjectile projectile)
        {
            _bulletPool.Release(projectile as Bullet);
        }
        protected override void Update()
        {
            base.Update();
            //Debug.Log(statemachine);
        }
    }
}
