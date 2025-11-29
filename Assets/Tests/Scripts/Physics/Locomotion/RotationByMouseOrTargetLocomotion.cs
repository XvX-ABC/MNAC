using System;
using Tests.Interaction;
using UnityEngine;

namespace Tests.TPhysics.Locomotion
{
    public class RotationByMouseOrTargetLocomotion : LocomotionModuleBase
    {
        WorldRotationLocomotion _b;
        Camera _camera;
        Vector3 _mouseScreenPosition;
        Vector3 _origin;
        IPositionTarget _target;
        public RotationByMouseOrTargetLocomotion(Camera camera)
        {
            Camera = camera;
            _b = new();
        }
        public Camera Camera
        {
            get => _camera;
            set
            {
                if (value == null)
                    throw new NullReferenceException(nameof(Camera));
                _camera = value;
            }
        }
        public Vector3 Origin { get => _origin; set => _origin = value; }
        public Vector3 MouseScreenPosition { get => _mouseScreenPosition; set => _mouseScreenPosition = value; }
        public IPositionTarget Target { get => _target; set => _target = value; }

        public override Context OnEnd(Context context)
        {
            return context;
        }

        public override Context OnStart(Context context)
        {
            return context;
        }

        public override Context OnUpdate(Context context)
        {
            var p = context.GroundPlane;
            _b.Origin = Vector3.ProjectOnPlane(_origin, p.normal);
            _b.TargetPos = _target == null ? CalculateMousePositionOn(p) : Vector3.ProjectOnPlane(_target.Position, p.normal);
            return _b.OnUpdate(context);
        }
        Vector3 CalculateMousePositionOn(Plane plane)
        {
            var ray = _camera.ScreenPointToRay(_mouseScreenPosition);
            if (plane.Raycast(ray, out var p))
                return ray.GetPoint(p);
            return Vector3.zero;
        }
    }
}
