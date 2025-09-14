using System.Collections.Generic;
using Tests.TPhysics.Environment;
using UnityEngine;

namespace Tests.TPhysics.Locomotion
{
    internal class RotationOnScreenLocomotion : LocomotionModuleBase
    {
        Quaternion _rotationOffset;
        Vector2 _bodyPos;
        Vector2 _targetPos;
        Camera _camera;
        Vector3 _origin;
        Vector2 _forward;
        public RotationOnScreenLocomotion()
        {
            _origin = new Vector3(0.5f, 0.5f);
            _forward = new Vector2(0, 0.5f);
            _rotationOffset = Quaternion.identity;
        }
        public Vector2 OriginalPos { get => _bodyPos; set => _bodyPos = value; }
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

        public override Context OnEnd(Context context)
        {
            return OnUpdate(context);
        }

        public override Context OnStart(Context context)
        {
            return OnUpdate(context);
        }

        Quaternion CalculateRotation(Context context)
        {
            var grounds = context.GroundDetector.Grounds;

            var groundNormal = CalculateNormalInGrounds(grounds);
            if (groundNormal == Vector3.zero)
                return world.rotation;

            return world.rotation * Quaternion.FromToRotation(world.Up, groundNormal);


            Vector3 CalculateNormalInGrounds(IReadOnlyList<Ground> grounds)
            {
                if (grounds.Count == 0)
                    return Vector3.zero;

                var result = Vector3.zero;
                foreach (var g in grounds)
                {
                    result += g.Normal;
                }
                return result / grounds.Count;
            }
        }
        public override Context OnUpdate(Context context)
        {
            if (_camera == null)
                return context;

            //var baseRotation = CalculateRotation(context);
            var tpos = _camera.ScreenToViewportPoint(_targetPos) - _origin;
            var bpos = _camera.ScreenToViewportPoint(_bodyPos) - _origin;
            var z = Quaternion.FromToRotation(_forward, tpos - bpos).eulerAngles.z;
            var r = Quaternion.Euler(0, -z, 0);
            if (_rotationOffset != Quaternion.identity)
                context.CurrentRotation = Quaternion.Slerp(context.CurrentRotation, _rotationOffset* r, Time.deltaTime * 15);
            else
                context.CurrentRotation = Quaternion.Slerp(context.CurrentRotation, r, Time.deltaTime * 15);
            return context;
        }
    }
}
