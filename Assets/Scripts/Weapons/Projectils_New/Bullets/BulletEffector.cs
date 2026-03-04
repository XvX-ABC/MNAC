using MNAC.Utilities.Blackboards;
using MNAC.Weapons.Projectiles;
using UnityEngine;

namespace MNAC.Weapons.Projectiles
{
    internal class BulletEffector : BulletComponent
    {
        [SerializeField]
        BulletEffect _movingEffect;
        [SerializeField]
        BulletHitParticleEffect _hitEffect;
        [SerializeField]
        LayerMask _collisionLayerMask;
        protected LayerMask collisionLayerMask { get => _collisionLayerMask; set => _collisionLayerMask = value; }
        private void OnEnable()
        {
            _movingEffect.gameObject.SetActive(true);
        }
        private void OnDisable()
        {
            _movingEffect.gameObject.SetActive(false);
        }
        private void FixedUpdate()
        {
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            if (blackboard.TryReadValue<LayerMask>(ProjectileFields.Hit_LayerMask, out var layerMask))
                _collisionLayerMask = layerMask;
            blackboard.RegisterFieldChangeAction<LayerMask>(ProjectileFields.Hit_LayerMask, WhenLayerMaskToHitChange);
            _hitEffect.Parent = this.owner.transform;
            //owner.HitAction += WhenHItObj;
            owner.HitAction_New += WhenHitObj;
            owner.StartMoveAction += WhenStartMove;
        }
        public override void Dispose()
        {
            blackboard.UnregisterFieldChangeAction<LayerMask>(ProjectileFields.Hit_LayerMask, WhenLayerMaskToHitChange);
            owner.HitAction -= WhenHItObj;
            owner.StartMoveAction -= WhenStartMove;
            base.Dispose();
        }
        void WhenLayerMaskToHitChange(FieldEventType type, LayerMask ov, LayerMask nv)
        {
            if (type == FieldEventType.Reading)
                return;
            collisionLayerMask = nv;
        }
        protected virtual void WhenHitObj(Vector3 prePos, Vector3 preForward, IProjectile projectile, GameObject obj)
        {
            if (!this.enabled)
                return;
            var ray = new Ray(prePos, preForward);
            var collider = owner._collider;
            if (Physics.SphereCast(ray, collider.radius, out var hitInfo, Mathf.Infinity, owner.LayerMaskToHit))
            {
                var point = hitInfo.point;
                var normal = hitInfo.normal;
                _hitEffect.transform.position = point;
                _hitEffect.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
                _hitEffect.transform.SetParent(hitInfo.collider.transform);
            }
            _movingEffect.Stop();
            _hitEffect.Play();
        }
        protected virtual void WhenHItObj(IProjectile projectile, GameObject hitObj)
        {
            if (!this.enabled)
                return;
            var collider = owner.GetComponent<CapsuleCollider>();
            if (collider != null)
            {
                var ray = new Ray(owner.transform.position, owner.transform.forward);
                if (Physics.SphereCast(ray, collider.radius, out var hitInfo, Mathf.Infinity, _collisionLayerMask))
                {
                    var point = hitInfo.point;
                    var normal = hitInfo.normal;
                    _hitEffect.transform.position = point;
                    _hitEffect.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
                    //_hitEffect.transform.SetParent(hitInfo.collider.transform);
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
