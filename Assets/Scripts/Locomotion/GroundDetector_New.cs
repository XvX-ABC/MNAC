using System.Collections.Generic;
using System.Text;
using Tests.Locomotion;
using UnityEngine;

namespace Locomotion
{
    public class GroundDetector_New : MonoBehaviour, IGroundDetector
    {
        public static Vector3 World_Up = Vector3.up;
        [SerializeField]
        LayerMask _groundMask;
        [SerializeField]
        float _maxSlope;
        GameObject _groundObj;
        Vector3 _groundNormal;
        Ground_New _ground;
        List<ContactPoint> _contactPoints;
        Rigidbody _rb;
        float _currentHeight;
        bool _collided;
        public IGround Ground
        {
            get => _collided ? _ground : null;
        }
        public bool Collided { get => _collided; }
        float IGroundDetector.CurrentHeight => _collided ? 0 : _currentHeight;
        bool IGroundDetector.AutoSample { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        bool IGroundDetector.TouchedGround => throw new System.NotImplementedException();


        Vector3 IGroundDetector.Normal => throw new System.NotImplementedException();

        Vector3 IGroundDetector.Point => throw new System.NotImplementedException();

        LayerMask IRayCollisionDetector.TargetLayerMask { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        float IRayCollisionDetector.Length { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        Vector3 IRayCollisionDetector.RelativeDirection { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        Vector3 IRayCollisionDetector.OriginOffset { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public GroundDetector_New()
        {
            _contactPoints = new();
            _ground = new Ground_New();
        }
        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }
        int FilterNormals()
        {
            var result = _contactPoints.Count;
            for (int i = 0; i < _contactPoints.Count; i++)
            {
                var p = _contactPoints[i];
                var normal = p.normal;
                var angle = Vector3.Angle(normal, World_Up);
                if (angle >= _maxSlope)
                {
                    _contactPoints.RemoveAt(i);
                    i--;
                    result--;
                }
            }
            return result;
        }
        Vector3 CalculateGroundNormal()
        {
            var result = Vector3.zero;
            foreach (var p in _contactPoints)
            {
                var normal = p.normal;
                result += normal;
            }
            return result / _contactPoints.Count;
        }
        private void OnCollisionEnter(Collision collision)
        {
            var obj = collision.gameObject;
            var layer = obj.layer;
            if (((1 << layer) & _groundMask) == 0)
                return;
            collision.GetContacts(_contactPoints);
            var quantity = FilterNormals();
            if (quantity <= 0)
            {
                _collided = false;
                return;
            }

            _groundNormal = CalculateGroundNormal();
            _groundObj = obj;
            _collided = true;
            _ground.normal = _groundNormal;
            _ground.obj = _groundObj;
        }
        private void OnCollisionStay(Collision collision)
        {
            var obj = collision.gameObject;
            var layer = obj.layer;
            if (((1 << layer) & _groundMask) == 0)
                return;
            collision.GetContacts(_contactPoints);
            var quantity = FilterNormals();
            if (quantity <= 0)
            {
                _collided = false;
                return;
            }
            else
                _collided = true;

            _groundNormal = CalculateGroundNormal();
            _groundObj = obj;
        }
        private void OnCollisionExit(Collision collision)
        {
            var obj = collision.gameObject;
            var layer = obj.layer;
            if (((1 << layer) & _groundMask) == 0)
                return;
            _groundObj = null;
            _groundNormal = Vector3.zero;
            _collided = false;

            _ground.obj = null;
        }
        void FixedUpdate()
        {
            if (_collided)
                return;
            if (Physics.Raycast(this.transform.position, -World_Up, out var hitInfo, Mathf.Infinity, _groundMask))
            {
                var pos = _rb.position;
                var point = hitInfo.point;
                _currentHeight = pos.y - point.y;
            }
        }
        void IGroundDetector.Sample()
        {
        }
    }
}
