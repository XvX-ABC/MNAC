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
        Ray _shootingRay;
        Action<IProjectile, GameObject> _hitAction;
        Rigidbody _rbody;
        public Ray ShootingRay { get => _shootingRay; set => _shootingRay = value; }

        public override Action<IProjectile, GameObject> HitAction { get => _hitAction; set => _hitAction = value; }

        void Awake()
        {
            _rbody = GetComponent<Rigidbody>();
        }
        private void OnCollisionEnter(Collision collision)
        {
            HitAction?.Invoke(this, collision.gameObject);
        }
        private void OnEnable()
        {
            _rbody.position = this.transform.position;
        }
        void FixedUpdate()
        {
            _rbody.MovePosition(_rbody.position + transform.forward * _speed * Time.fixedDeltaTime);
        }
    }
}
