using System;
using Tests.Interaction;
using UnityEngine;

namespace Tests.Weapons_New.Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class Bullet : Projectile, IBullet
    {
        public delegate void HitActionDelegate(IProjectile projectile, GameObject obj);
        [SerializeField]
        float _speed;
        [SerializeField]
        float _damagePoint;
        Ray _shootingRay;
        Rigidbody _rbody;
        SphereCollider _collider;
        Action<IProjectile> _startMoveAction;
        HitActionDelegate _hitAction;
        public Ray ShootingRay { get => _shootingRay; set => _shootingRay = value; }

        public Action<IProjectile> StartMoveAction { get => _startMoveAction; set => _startMoveAction = value; }
        internal HitActionDelegate HitAction { get => _hitAction; set => _hitAction = value; }

        protected override void Awake()
        {
            base.Awake();
            _rbody = GetComponent<Rigidbody>();
            _collider = GetComponent<SphereCollider>();
            blackboard.TryRegisterField(BulletComponent.OwnerBullet, this);
        }
        protected virtual void OnTriggerEnter(Collider other)
        {


            _hitAction?.Invoke(this, other.gameObject);
            Damage(other.gameObject);
            EndAction();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            _rbody.position = this.transform.position;
            _rbody.rotation = this.transform.rotation;
        }
        protected override void Start()
        {
            _startMoveAction?.Invoke(this);
        }
        void FixedUpdate()
        {
            var originPos = _rbody.position;
            var length = _speed * Time.fixedDeltaTime;
            var nextPos = _rbody.position + transform.forward * length;
            if (Physics.SphereCast(originPos, _collider.radius, transform.forward, out var hit, length, layerMaskToHit))
            {
                nextPos = hit.point;
            }
            _rbody.MovePosition(nextPos);
        }
        void Damage(GameObject obj)
        {
            if (InteractionHelper.CheckFriendly(ownerTeamMask, obj))
                return;

            if (!obj.TryGetComponent<IDamageable>(out var d) || !d.HP.IsAlive)
                return;
            d.HP.ReceivePoint(_damagePoint);
        }

    }
}
