using System;
using UnityEngine;

namespace Tests.Weapons.Projectiles
{
    public interface IMissile : IProjectile
    {
        public ITarget Target { get; set; }
    }
    public interface IMissileDefines : IProjectlieDefines
    {
        public float MaxAngle { get; }
    }
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Missile : MonoBehaviour, IMissile
    {
        //public GameObject Object => this.gameObject;
        IMissileDefines _defines;
        ITarget _target;
        Action<IProjectile, GameObject> _hitAction;
        public ITarget Target { get => _target; set => _target = value; }
        public Action<IProjectile, GameObject> HitAction { get => _hitAction; set => _hitAction = value; }
        void Awake()
        {
            _defines = GetComponent<IMissileDefines>() ?? throw new ComponentCantFoundException(this.gameObject, typeof(IMissileDefines));
        }
        private void OnCollisionEnter(Collision collision)
        {
            _hitAction?.Invoke(this, collision.gameObject);
        }
        Vector3 CalculateNextPosition(float deltaTime)
        {
            return this.transform.position += this.transform.forward * _defines.Speed * deltaTime;
        }
        Quaternion CalculateNextRotation(float deltaTime)
        {
            var targetPos = _target.Locomotion.Position;
            var currentPos = this.transform.position;
            var tv = targetPos - currentPos;
            var forward = this.transform.forward;
            var v = Vector3.Cross(tv, forward);
            

            return Quaternion.identity;
        }
    }
}