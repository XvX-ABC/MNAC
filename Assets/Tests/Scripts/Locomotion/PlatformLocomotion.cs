using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
namespace Tests.Locomotion
{
    public class PlatformLocomotion : MonoBehaviour
    {
        class Platform
        {
            public GameObject Obj;
            public Vector3 OldPos;
        }
        public class A : IModule
        {
            PlatformLocomotion _l;

            public A(PlatformLocomotion l)
            {
                _l = l;
            }

            public void OnUpdate(Context context)
            {
                if (_l._parentConstraint.constraintActive)
                {
                    var trans = _l._platform.Obj.transform;
                    var rb = _l._rb;
                    var currentPos = trans.TransformPoint(_l._localPos);
                    var d = currentPos - _l._platform.OldPos;
                    var rd = trans.TransformDirection(_l._localRotation);


                    rb.MovePosition(rb.position + d);
                    //rb.MoveRotation(Quaternion.LookRotation(rd));
                    context.Rotation = Quaternion.LookRotation(rd);


                    _l._platform.OldPos = currentPos;
                }
            }
        }
        public class B : IModule
        {
            PlatformLocomotion _l;

            public B(PlatformLocomotion l)
            {
                _l = l;
            }

            public void OnUpdate(Context context)
            {
                if (_l._parentConstraint.constraintActive)
                {
                    var trans = _l._platform.Obj.transform;
                    var p = _l._platform;
                    _l._localPos = p.Obj.transform.InverseTransformPoint(context.Position);
                    _l._localRotation = trans.InverseTransformDirection(context.Rotation * Vector3.forward);
                }
            }
        }
        [SerializeField]
        LayerMask _platformMask;
        [SerializeField]
        float _platformSlope;
        List<ContactPoint> _contactPoints;
        Rigidbody _rb;
        ParentConstraint _parentConstraint;
        Platform _platform;
        ushort _idx;
        Vector3 _localPos;
        Vector3 _localRotation;
        public A AM;
        public B BM;
        public PlatformLocomotion()
        {
            _contactPoints = new();
            AM = new(this);
            BM = new(this);
        }
        private void Awake()
        {
            _parentConstraint = GetComponent<ParentConstraint>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ParentConstraint));
            _rb = GetComponent<Rigidbody>();
            _platform = new();
        }
        public void OnCollisionEnter(Collision collision)
        {
            var obj = collision.gameObject;
            var layer = obj.layer;


            if ((1 << layer) != _platformMask)
                return;


            collision.GetContacts(_contactPoints);
            var pass = false;
            foreach (var p in _contactPoints)
            {
                var normal = p.normal;
                var angle = Vector3.Angle(normal, Vector3.up);
                if (angle < _platformSlope)
                {
                    pass = true;
                    break;
                }
            }
            if (!pass)
                return;


            _platform.Obj = obj;
            _localPos = obj.transform.InverseTransformPoint(_rb.position);
            _localRotation = obj.transform.InverseTransformDirection(_rb.rotation * Vector3.forward);

            _platform.OldPos = _rb.position;
            _idx = (byte)_parentConstraint.AddSource(new ConstraintSource() { sourceTransform = _platform.Obj.transform, weight = 1 });
            var scale = obj.transform.localScale;
            _parentConstraint.SetTranslationOffset(_idx, new Vector3(_localPos.x * scale.x, _localPos.y * scale.y, _localPos.z * scale.z));
            _parentConstraint.SetRotationOffset(_idx, Quaternion.LookRotation(_localRotation).eulerAngles);
            _parentConstraint.constraintActive = true;


            _contactPoints.Clear();
        }

        private void OnCollisionStay(Collision collision)
        {
            var obj = collision.gameObject;
            var layer = obj.layer;
            if ((1 << layer) != _platformMask || !_parentConstraint.constraintActive)
                return;


            collision.GetContacts(_contactPoints);
            var pass = false;
            foreach (var p in _contactPoints)
            {
                var normal = p.normal;
                var angle = Vector3.Angle(normal, Vector3.up);
                if (angle < _platformSlope)
                {
                    pass = true;
                    break;
                }
            }
            if (!pass)
                return;


            _localPos = _platform.Obj.transform.InverseTransformPoint(_rb.position);
            var scale = _platform.Obj.transform.localScale;
            _parentConstraint.SetTranslationOffset(_idx, new Vector3(_localPos.x * scale.x, _localPos.y * scale.y, _localPos.z * scale.z));
            _parentConstraint.SetRotationOffset(_idx, Quaternion.LookRotation(_localRotation).eulerAngles);


            _contactPoints.Clear();
        }
        private void OnCollisionExit(Collision collision)
        {
            var obj = collision.gameObject;
            var layer = obj.layer;
            if ((1 << layer) != _platformMask || !_parentConstraint.constraintActive)
                return;


            collision.GetContacts(_contactPoints);
            var pass = false;
            foreach (var p in _contactPoints)
            {
                var normal = p.normal;
                var angle = Vector3.Angle(normal, Vector3.up);
                if (angle < _platformSlope)
                {
                    pass = true;
                    break;
                }
            }
            if (pass)
                return;


            _parentConstraint.constraintActive = false;
            _parentConstraint.RemoveSource(0);
            _platform.Obj = null;


            _contactPoints.Clear();
        }
    }
}