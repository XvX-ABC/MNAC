using System;
using MNAC.Utilities.Blackboards;
using UnityEngine;

namespace MNAC.Weapons.Projectiles
{
    internal class BulletComponent : ProjectileComponent
    {
        public static readonly Guid OwnerBullet;
        static BulletComponent()
        {
            OwnerBullet = Guid.NewGuid();
        }
        protected Bullet owner;
        protected Ray shootingRay { get => owner.ShootingRay; }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryReadValueOrThrowException(OwnerBullet, out owner);
        }
    }
}
