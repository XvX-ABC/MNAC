using Locomotion;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Tests.Locomotion
{
    public class GroundDetector : MonoBehaviour, IGroundDetector
    {
        public static Vector3 World_Up = Vector3.up;
        [SerializeField]
        LayerMask _groundMask;
        [SerializeField]
        float _maxSlope;
        Ground_New _ground;
        List<ContactPoint> _contactPoints;
        Rigidbody _rb;
        float _currentHeight;
        //float _currentGroundHeight;
        public IGround CollidedGround
        {
            get => _ground.collided ? _ground : null;
        }
        public bool Collided { get => _ground.collided; }
        float IGroundDetector.Distance => _ground.collided ? 0 : _currentHeight;
        public float GroundHeight => _ground.height;
        public GroundDetector()
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
            var sb = new StringBuilder();
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
                    continue;
                }
                sb.AppendLine($"[ {i} ] -> {angle} ");
            }
            //Debug.Log(sb.ToString());
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
            return;
            var obj = collision.gameObject;
            var layer = obj.layer;
            if ((1 << layer & _groundMask) == 0)
                return;

            _ground.height = obj.transform.position.y;

            collision.GetContacts(_contactPoints);
            var quantity = FilterNormals();
            if (quantity <= 0)
            {
                _ground.collided = false;
                return;
            }

            _ground.normal = CalculateGroundNormal();
            _ground.obj = obj;
            _ground.collided = true;
        }
        private void OnCollisionStay(Collision collision)
        {
            return;
            var obj = collision.gameObject;
            var layer = obj.layer;
            if ((1 << layer & _groundMask) == 0)
                return;

            _ground.height = obj.transform.position.y;

            collision.GetContacts(_contactPoints);
            var quantity = FilterNormals();
            if (quantity <= 0)
            {
                _ground.collided = false;
                return;
            }
            else
                _ground.collided = true;

            _ground.normal = CalculateGroundNormal();
            _ground.obj = obj;

        }
        private void OnCollisionExit(Collision collision)
        {
            return;
            var obj = collision.gameObject;
            var layer = obj.layer;
            if ((1 << layer & _groundMask) == 0)
                return;
            _ground.normal = Vector3.zero;
            _ground.collided = false;

        }
        void FixedUpdate()
        {
            return;
            if (_ground.collided)
                return;
            if (Physics.Raycast(transform.position, -World_Up, out var hitInfo, Mathf.Infinity, _groundMask))
            {
                var obj = hitInfo.collider.gameObject;
                var pos = _rb.position;
                var point = hitInfo.point;
                _currentHeight = pos.y - point.y;
                _ground.height = obj.transform.position.y;
            }
        }

        public void OnFixedUpdate()
        {
            if (_ground.collided)
                return;
            if (Physics.Raycast(transform.position, -World_Up, out var hitInfo, Mathf.Infinity, _groundMask))
            {
                var obj = hitInfo.collider.gameObject;
                var pos = _rb.position;
                var point = hitInfo.point;
                _currentHeight = pos.y - point.y;
                _ground.height = obj.transform.position.y;
            }
        }

        public void OnColliderEnter(CollisionContext context)
        {
            var collision = context.Collision;
            var obj = collision.gameObject;
            var layer = obj.layer;
            if ((1 << layer & _groundMask) == 0)
                return;

            _ground.height = obj.transform.position.y;

            collision.GetContacts(_contactPoints);
            var quantity = FilterNormals();
            if (quantity <= 0)
            {
                _ground.collided = false;
                return;
            }

            _ground.normal = CalculateGroundNormal();
            _ground.obj = obj;
            _ground.collided = true;
        }

        public void OnColliderStay(CollisionContext context)
        {
            var collision = context.Collision;
            var obj = collision.gameObject;
            var layer = obj.layer;
            if ((1 << layer & _groundMask) == 0)
                return;

            _ground.height = obj.transform.position.y;

            collision.GetContacts(_contactPoints);
            var quantity = FilterNormals();
            if (quantity <= 0)
            {
                _ground.collided = false;
                return;
            }
            else
                _ground.collided = true;

            _ground.normal = CalculateGroundNormal();
            _ground.obj = obj;

        }

        public void OnColliderExit(CollisionContext context)
        {
            var collision = context.Collision;
            var obj = collision.gameObject;
            var layer = obj.layer;
            if ((1 << layer & _groundMask) == 0)
                return;
            _ground.normal = Vector3.zero;
            _ground.collided = false;
        }
    }
}
