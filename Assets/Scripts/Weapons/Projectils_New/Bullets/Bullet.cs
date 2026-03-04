using System;
using MNAC.Interaction;
using UnityEngine;

namespace MNAC.Weapons.Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Bullet : Projectile, IBullet
    {
        public delegate void HitActionDelegate(IProjectile projectile, GameObject obj);
        [SerializeField]
        float _speed;
        [SerializeField]
        float _damagePoint;
        [SerializeField]
        float _maxFlyingDistance;
        Vector3 _prePos;
        Vector3 _preForward;
        Ray _shootingRay;
        internal Rigidbody _rbody;
        internal CapsuleCollider _collider;
        Action<IProjectile> _startMoveAction;
        HitActionDelegate _hitAction;
        Action<Vector3, Vector3, IProjectile, GameObject> _hitAction_New;
        float _currentDistance;
        public Ray ShootingRay { get => _shootingRay; set => _shootingRay = value; }

        public Action<IProjectile> StartMoveAction { get => _startMoveAction; set => _startMoveAction = value; }
        internal HitActionDelegate HitAction { get => _hitAction; set => _hitAction = value; }
        public Action<Vector3, Vector3, IProjectile, GameObject> HitAction_New { get => _hitAction_New; set => _hitAction_New = value; }

        protected override void Awake()
        {
            base.Awake();
            _rbody = GetComponent<Rigidbody>();
            _collider = GetComponent<CapsuleCollider>();
            blackboard.TryRegisterField(BulletComponent.OwnerBullet, this);
        }
        protected virtual void OnTriggerEnter(Collider other)
        {

            _hitAction_New?.Invoke(_prePos, _preForward, this, other.gameObject);
            _hitAction?.Invoke(this, other.gameObject);
            Damage(other.gameObject);
            EndAction();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            _rbody.position = this.transform.position;
            _rbody.rotation = this.transform.rotation;
            _currentDistance = 0;
        }
        void FixedUpdate()
        {
            if (_currentDistance >= _maxFlyingDistance)
            {
                EndAction();
                return;
            }
            var originPos = _prePos = _rbody.position;
            var length = _speed * Time.fixedDeltaTime;
            var f = _preForward = _rbody.rotation * Vector3.forward;
            var nextPos = _rbody.position + f * length;
            Debug.DrawLine(originPos, originPos + f * 100, Color.green);
            if (Physics.SphereCast(originPos, _collider.radius, f, out var hit, length, layerMaskToHit))
            {
                nextPos = hit.point;
            }
            //_rbody.MovePosition(nextPos);
            this.transform.position = nextPos;
            _currentDistance += length;
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
