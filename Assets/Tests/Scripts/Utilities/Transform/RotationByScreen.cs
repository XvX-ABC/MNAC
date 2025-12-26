using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tests.Utilities
{

    public class RotationByScreen
    {
        Quaternion _rotationOffset;
        Vector2 _originalPos;
        Vector2 _targetPos;
        Camera _camera;
        Vector3 _origin;
        Vector2 _forward;
        public RotationByScreen()
        {
            _origin = new Vector3(0.5f, 0.5f);
            _forward = new Vector2(0, 0.5f);
            _rotationOffset = Quaternion.identity;
            _originalPos = _targetPos = TransformHelper.InvalidPosition_V2;
            _camera = null;
        }
        public Vector2 OriginalPos { get => _originalPos; set => _originalPos = value; }
        public Vector2 TargetPos { get => _targetPos; set => _targetPos = value; }
        public Camera Camera
        {
            get => _camera;
            set
            {
                _camera = value;
            }
        }
        public Quaternion RotationOffset { get => _rotationOffset; set => _rotationOffset = value; }
        public Quaternion Calculate()
        {
            if (_camera == null || _originalPos == TransformHelper.InvalidPosition_V2 || _targetPos == TransformHelper.InvalidPosition_V2)
                return Quaternion.identity;
            var tpos = _camera.ScreenToViewportPoint(_targetPos) - _origin;
            var bpos = _camera.ScreenToViewportPoint(_originalPos) - _origin;
            var z = Quaternion.FromToRotation(_forward, tpos - bpos).eulerAngles.z;
            var r = Quaternion.Euler(0, -z, 0);
            return _rotationOffset == Quaternion.identity ? r : _rotationOffset * r;
        }
    }
}
