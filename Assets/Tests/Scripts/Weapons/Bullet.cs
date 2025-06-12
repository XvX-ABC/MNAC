using Assets.Scripts.Utilities;
using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using System;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace Tests.Weapons
{
    public interface IBullet : IProjectile
    {
        public Action<IProjectile, GameObject> DisableAction { get; set; }
        public Ray ShootingRay { get; set; }
    }
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Bullet : MonoBehaviour, IBullet
    {
        private IProjectileDefinitions _definition;
        private Action<IProjectile, GameObject> _hitAction;
        private Action<IProjectile, GameObject> _disableAction;
        BulletEffects _effects;
        Rigidbody _rb;
        float _speed;
        Ray _shootingRay;
        public Action<IProjectile, GameObject> HitAction { get => _hitAction; set => _hitAction = value; }
        public Action<IProjectile, GameObject> DisableAction { get => _disableAction; set => _disableAction = value; }
        public bool Enabled { get => enabled; set => enabled = value; }
        public Ray ShootingRay
        {
            get => _shootingRay;
            set
            {
                if (_effects != null)
                    _effects.shootingRay = value;
                _shootingRay = value;
            }
        }


        private void Awake()
        {
            _definition = GetComponent<IProjectileDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IProjectileDefinitions));
            _rb = GetComponent<Rigidbody>();
            _effects = GetComponent<BulletEffects>();
        }
        void Start()
        {
            if (_effects != null)
            {
                var timeline = _effects.timeline;
                timeline.AddPointEvent(1, _ =>
                {
                    _disableAction?.Invoke(this, this.gameObject);
                    this.enabled = false;
                });
            }
        }
        void OnEnable()
        {
            _speed = _definition.Speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(MethodInfo.GetCurrentMethod().Name + "," + this.gameObject.name);
            var obj = other.gameObject;
            _hitAction?.Invoke(this, obj);
        }
        private void OnCollisionEnter(Collision collision)
        {
            _speed = 0;
            var obj = collision.gameObject;
            _hitAction?.Invoke(this, obj);
            if (_effects == null)
                this.enabled = false;
        }
        void FixedUpdate()
        {
            _rb.MovePosition(_rb.position + this.transform.forward * _speed * Time.fixedDeltaTime);
        }
    }
}