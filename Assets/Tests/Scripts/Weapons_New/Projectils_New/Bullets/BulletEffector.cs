using Tests.Utilities.Blackboards;
using Tests.Weapons_New.Projectiles;
using UnityEngine;

namespace Tests.Weapons.Projectiles_New
{
    internal class BulletEffector : BulletComponent
    {
        [SerializeField]
        BulletEffect _movingEffect;
        [SerializeField]
        BulletHitParticleEffect _hitEffect;
        [SerializeField]
        LayerMask _collisionLayer;
        private void OnEnable()
        {
            _movingEffect.gameObject.SetActive(true);
            //_hitEffect.gameObject.SetActive(true);
        }
        private void OnDisable()
        {
            _movingEffect.gameObject.SetActive(false);
            //_hitEffect.gameObject.SetActive(false);
        }
        private void FixedUpdate()
        {
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            _hitEffect.Parent = this.owner.transform;
            owner.HitAction += WhenHItObj;
            owner.StartMoveAction += WhenStartMove;
        }
        public override void Dispose()
        {
            base.Dispose();
            owner.HitAction -= WhenHItObj;
            owner.StartMoveAction -= WhenStartMove;
        }
        protected virtual void WhenHItObj(IProjectile projectile, GameObject hitObj)
        {
            if (!this.enabled)
                return;
            var collider = owner.GetComponent<CapsuleCollider>();
            if (collider != null)
            {
                if (Physics.SphereCast(shootingRay, collider.radius, out var hitInfo, Mathf.Infinity, _collisionLayer))
                {
                    var point = hitInfo.point;
                    var normal = hitInfo.normal;
                    _hitEffect.transform.position = point;
                    _hitEffect.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
                }
            }
            _movingEffect.Stop();
            _hitEffect.Play();
        }
        protected void WhenStartMove(IProjectile projectile)
        {
            if (!this.enabled)
                return;
            _movingEffect.Play();
        }
    }
}
