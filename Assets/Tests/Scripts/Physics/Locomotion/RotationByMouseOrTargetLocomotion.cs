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
        public Vector3 Origin { get => _b.Origin; set => _b.Origin = value; }
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
            var p = context.WorldPlane;
            _b.TargetPos = _target == null ? CalculateMousePositionInWorld(p) : Vector3.ProjectOnPlane(_target.Position, p.normal);
            return _b.OnUpdate(context);
        }
        Vector3 CalculateMousePositionInWorld(Plane plane)
        {
            var ray = _camera.ScreenPointToRay(_mouseScreenPosition);
            if (plane.Raycast(ray, out var p))
                return ray.GetPoint(p);
            return Vector3.zero;
        }
    }
}
