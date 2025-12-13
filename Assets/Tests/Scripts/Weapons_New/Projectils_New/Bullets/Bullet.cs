using System;
using Tests.Interaction;
using Tests.Weapons_New.Projectiles;
using UnityEngine;

namespace Tests.Weapons_New.Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Bullet : Projectile, IBullet
    {
        public delegate void HitActionDelegate(IProjectile projectile, GameObject obj);
        [SerializeField]
        float _speed;
        [SerializeField]
        float _damagePoint;
        Ray _shootingRay;
        Rigidbody _rbody;
        Action<IProjectile> _startMoveAction;
        HitActionDelegate _hitAction;
        public Ray ShootingRay { get => _shootingRay; set => _shootingRay = value; }

        public Action<IProjectile> StartMoveAction { get => _startMoveAction; set => _startMoveAction = value; }
        internal HitActionDelegate HitAction { get => _hitAction; set => _hitAction = value; }

        protected override void Awake()
        {
            base.Awake();
            _rbody = GetComponent<Rigidbody>();
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
            _rbody.MovePosition(_rbody.position + transform.forward * _speed * Time.fixedDeltaTime);
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
