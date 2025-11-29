using System;
using Tests.Weapons_New.Projectiles;
using UnityEngine;

namespace Tests.Weapons.Projectiles_New
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    internal class Bullet : Projectile, IBullet
    {
        [SerializeField]
        float _speed;
        [SerializeField]
        LayerMask _layerMaskToHit;
        Ray _shootingRay;
        Action<IProjectile, GameObject> _hitAction;
        Rigidbody _rbody;
        Action<IProjectile> _startMoveAction;
        public Ray ShootingRay { get => _shootingRay; set => _shootingRay = value; }

        public override Action<IProjectile, GameObject> HitAction { get => _hitAction; set => _hitAction = value; }
        public Action<IProjectile> StartMoveAction { get => _startMoveAction; set => _startMoveAction = value; }
        public LayerMask LayerMaskToHit
        {
            get => _layerMaskToHit;
            set
            {
                _layerMaskToHit = value;
                blackboard.TryRegisterFieldOrWriteValue<LayerMask>(ProjectileFields.Hit_LayerMask, _layerMaskToHit);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _rbody = GetComponent<Rigidbody>();
            blackboard.TryRegisterField(BulletComponent.OwnerBullet, this);
            LayerMaskToHit = _layerMaskToHit;
        }
        private void OnTriggerEnter(Collider other)
        {
            HitAction?.Invoke(this, other.gameObject);
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
    }
}
