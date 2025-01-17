using System;
using System.Reflection;
using UnityEngine;

namespace Tests.Weapons
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Bullet : MonoBehaviour, IProjectile
    {
        IProjectlieDefines _defines;
        Action<IProjectile, GameObject> _hitAction;

        public Action<IProjectile, GameObject> HitAction { get => _hitAction; set => _hitAction = value; }
        public bool Enabled { get => enabled; set => enabled = value; }
        void Awake()
        {
            _defines = GetComponent<IProjectlieDefines>() ?? throw new ComponentCantFoundException(this.gameObject, typeof(IProjectlieDefines));
        }
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(MethodInfo.GetCurrentMethod().Name);
            var obj = other.gameObject;
            _hitAction?.Invoke(this, obj);
        }
        void Update()
        {
            if (!enabled)
                return;
            this.transform.position += this.transform.forward * _defines.Speed * Time.deltaTime;
        }
    }
}