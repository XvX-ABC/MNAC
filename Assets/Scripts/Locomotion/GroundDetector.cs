using System;
using Tests.Environment;
using UnityEngine;

namespace Locomotion
{
    public class GroundDetector : MonoBehaviour, IGroundDetector
    {
        [SerializeField]
        LayerMask _targetLayerMask;
        [SerializeField]
        float _length;
        [SerializeField]
        Vector3 _relativeDirection;
        [SerializeField]
        Vector3 _originOffset;
        [SerializeField]
        bool _touchedGround;
        bool _autoSample;
        bool _enabledHeightSample;

        Vector3 _center;
        Vector3 _origin;
        Vector3 _normal;
        Vector3 _point;
        float _height;

        public LayerMask TargetLayerMask { get => _targetLayerMask; set => _targetLayerMask = value; }
        public float Length { get => _length; set => _length = value; }
        public Vector3 RelativeDirection { get => _relativeDirection; set => _relativeDirection = value; }
        public Vector3 OriginOffset { get => _originOffset; set => _originOffset = value; }


        public bool AutoSample { get => _autoSample; set => _autoSample = value; }
        public bool EnabledHeightSample
        {
            get => _enabledHeightSample;
            set => _enabledHeightSample = value;
        }


        public bool TouchedGround { get => _touchedGround; }
        public Vector3 Normal { get => _normal; }
        public Vector3 Point { get => _point; }
        public float Distance { get => _height; }

        public IGround CollidedGround => throw new NotImplementedException();

        public float GroundHeight => throw new NotImplementedException();

        void Start()
        {
            var collider = GetComponent<Collider>();
            if (collider is CapsuleCollider ccollider)
                _center = ccollider.center;
            else if (collider is BoxCollider bcollider)
                _center = bcollider.center;
        }
        void Update()
        {
            _origin = this.transform.position + _center;
        }
        public void Sample()
        {
            if (Physics.Raycast(new() { origin = _origin, direction = this.transform.rotation * _relativeDirection }, out var hitInfo, Mathf.Infinity, _targetLayerMask))
            {
                _height = (this.transform.position - hitInfo.point).magnitude;
                _touchedGround = (_origin - hitInfo.point).magnitude <= _length;
                //_isOnGround = true;
                _point = hitInfo.point;
                _normal = hitInfo.normal;
            }
            else
            {
                _normal = Vector3.zero;
                _touchedGround = false;
                _height = 0;
                _point = Vector3.positiveInfinity;
            }
        }
        void FixedUpdate()
        {
            if (_autoSample)
                Sample();
        }
        void OnDrawGizmos()
        {
            var pos = _origin;
            var rotation = this.transform.rotation;
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(pos, pos + (rotation * _relativeDirection) * Length);
        }

    }
}
