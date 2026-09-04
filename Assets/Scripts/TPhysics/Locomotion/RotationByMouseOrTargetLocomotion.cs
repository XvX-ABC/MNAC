using MNAC.Interaction;
using UnityEngine;

namespace MNAC.TPhysics.Locomotion
{
    /// 优先面向 Target 位置；无目标时回退为面向鼠标在水平面上的落点。
    public class RotationByMouseOrTargetLocomotion : LocomotionModuleBase
    {
        readonly RotationLocomotion _base = new();
        Camera _camera;
        Vector3 _mouseScreenPosition;
        IPositionTarget _target;

        public RotationByMouseOrTargetLocomotion(Camera camera)
        {
            Camera = camera;
        }

        public Camera Camera
        {
            get => _camera;
            set => _camera = value;
        }

        public Vector3 MouseScreenPosition { get => _mouseScreenPosition; set => _mouseScreenPosition = value; }

        public IPositionTarget Target { get => _target; set => _target = value; }

        public override Context OnUpdate(Context context)
        {
            var plane = context.WorldPlane;
            if (_target == null)
            {
                if (_camera == null)
                    return context;
                _base.TargetPos = Vector3.ProjectOnPlane(CalculateMousePositionOn(plane), plane.normal);
            }
            else
                _base.TargetPos = _target.Position;

            _base.Origin = Vector3.ProjectOnPlane(context.CurrentPosition, plane.normal);
            return _base.OnUpdate(context);
        }

        Vector3 CalculateMousePositionOn(Plane plane)
        {
            var ray = _camera.ScreenPointToRay(_mouseScreenPosition);
            return plane.Raycast(ray, out var distance) ? ray.GetPoint(distance) : Vector3.zero;
        }
    }
}
