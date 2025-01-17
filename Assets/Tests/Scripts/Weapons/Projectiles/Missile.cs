using System;
using UnityEditorInternal;
using UnityEngine;

namespace Tests.Weapons.Projectiles
{
    public interface IMissile : IProjectile
    {
        public ITarget Target { get; set; }
    }
    public interface IMissileDefines : IProjectlieDefines
    {
        public float AngularAngle { get; }
        public float MaxAngle { get; }
    }
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Missile : MonoBehaviour, IMissile
    {
        //public GameObject Object => this.gameObject;
        IMissileDefines _defines;
        ITarget _target;
        [SerializeField]
        float _acceleratedAngle;

        Action<IProjectile, GameObject> _hitAction;
        public ITarget Target { get => _target; set => _target = value; }
        public Action<IProjectile, GameObject> HitAction { get => _hitAction; set => _hitAction = value; }
        public bool Enabled { get => enabled; set => enabled = value; }
        void Awake()
        {
            _defines = GetComponent<IMissileDefines>() ?? throw new ComponentCantFoundException(this.gameObject, typeof(IMissileDefines));

            _target = GetComponent<ITarget>() ?? throw new ComponentCantFoundException(this.gameObject, typeof(ITarget));
        }
        private void OnCollisionEnter(Collision collision)
        {
            _hitAction?.Invoke(this, collision.gameObject);
            var obj = collision.gameObject;
            if (obj == _target.Obj)
                enabled = false;
        }
        void Update()
        {
            if (!enabled)
                return;
            this.transform.position = CalculateNextPosition(Time.deltaTime);
            this.transform.rotation = CalculateNextRotation(Time.deltaTime);
        }
        void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;
            var pos = this.transform.position;
            var targetPos = _target.Locomotion.Position;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos, targetPos);
        }
        Vector3 CalculateNextPosition(float deltaTime)
        {
            return this.transform.position += this.transform.forward * _defines.Speed * deltaTime;
        }
        Quaternion CalculateNextRotation(float deltaTime)
        {
            var currentPos = this.transform.position;
            var targetPos = _target.Locomotion.Position;

            var tv = targetPos - currentPos;

            var rotation = this.transform.rotation;
            var trotation = Quaternion.LookRotation(tv.normalized);

            var angle = Quaternion.Angle(rotation, trotation);

            if (angle <= 1)
            {
                _acceleratedAngle = angle;
                return rotation;
            }
            else
            {
                var expectedAngle = Mathf.Min(_acceleratedAngle + Mathf.Min(angle, _defines.AngularAngle * deltaTime), _defines.MaxAngle);
                _acceleratedAngle = expectedAngle;
                return Quaternion.Lerp(rotation, trotation, expectedAngle / angle);
            }
        }
    }
}