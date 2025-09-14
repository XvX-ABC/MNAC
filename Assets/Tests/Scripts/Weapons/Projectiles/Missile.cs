using System;
using UnityEngine;

namespace Tests.Weapons.Projectiles
{
    public interface IMissile : IProjectile
    {
        public ITarget_Obsolete Target { get; set; }
    }
    public interface IMissileDefinitions : IProjectileDefinitions
    {
        public float AngularSpeed { get; }
        public float MaxAngle { get; }
    }
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Missile : MonoBehaviour, IMissile
    {
        //public GameObject Object => this.gameObject;
        Rigidbody _rb;
        IMissileDefinitions _definition;
        ITarget_Obsolete _target;
        [SerializeField]
        float _acceleratedAngle;
        [SerializeField]
        AnimationCurve _curve;

        Action<IProjectile, GameObject> _hitAction;
        public ITarget_Obsolete Target
        {
            get => _target;
            set
            {
                _target = value;
            }
        }
        public Action<IProjectile, GameObject> HitAction { get => _hitAction; set => _hitAction = value; }
        public bool Enabled { get => enabled; set => enabled = value; }
        void Awake()
        {
            _definition = GetComponent<IMissileDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IMissileDefinitions));
            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = false;
            _target = GetComponent<ITarget_Obsolete>();
            //_rb.isKinematic = true;
        }
        private void OnEnable()
        {
            _rb.isKinematic = false;
        }
        private void OnDisable()
        {
            _rb.isKinematic = true;
        }
        private void OnCollisionEnter(Collision collision)
        {
            _hitAction?.Invoke(this, collision.gameObject);
            var obj = collision.gameObject;
            if (obj == _target?.Obj)
            {
                //Debug.Log($"Missile {obj.name} hit the targget '{obj.name}'");
                enabled = false;
            }
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
            if (_target == null)
                return;
            var targetPos = _target.Position;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos, targetPos);
        }
        Vector3 CalculateNextPosition(float deltaTime)
        {
            return this.transform.position += this.transform.forward * _definition.Speed * deltaTime;
        }
        Quaternion CalculateNextRotation(float deltaTime)
        {
            var currentPos = this.transform.position;
            var targetPos = _target.Position;

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
                var expectedAngle = Mathf.Min(_acceleratedAngle + Mathf.Min(angle, _definition.AngularSpeed * deltaTime), _definition.MaxAngle);
                _acceleratedAngle = expectedAngle;
                return Quaternion.Slerp(rotation, trotation, expectedAngle / angle);
            }
        }
    }
}