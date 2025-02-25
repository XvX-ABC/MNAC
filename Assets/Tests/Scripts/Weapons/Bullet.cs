using System;
using System.Reflection;
using UnityEngine;

namespace Tests.Weapons
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Bullet : MonoBehaviour, IProjectile
    {
        private IProjectileDefinitions _definition;
        private Action<IProjectile, GameObject> _hitAction;

        public Action<IProjectile, GameObject> HitAction { get => _hitAction; set => _hitAction = value; }
        public bool Enabled { get => enabled; set => enabled = value; }

        private void Awake()
        {
            _definition = GetComponent<IProjectileDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IProjectileDefinitions));
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(MethodInfo.GetCurrentMethod().Name);
            var obj = other.gameObject;
            _hitAction?.Invoke(this, obj);
        }

        private void Update()
        {
            if (!enabled)
                return;
            this.transform.position += this.transform.forward * _definition.Speed * Time.deltaTime;
        }
    }
}