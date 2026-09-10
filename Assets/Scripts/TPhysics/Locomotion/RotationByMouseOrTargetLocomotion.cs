using MNAC.Interaction;
using System;
using UnityEngine;

namespace MNAC.TPhysics.Locomotion
{
    /// 优先面向 Target 位置；无目标时回退为面向鼠标在水平面上的落点。
    [Serializable]
    public class RotationByMouseOrTargetLocomotion : LocomotionModuleBase
    {
        [NonSerialized] RotationLocomotion _base;
        [SerializeField] Camera _camera;
        [SerializeField] Vector3 _mouseScreenPosition;
        [NonSerialized] IPositionTarget _target;

        public RotationByMouseOrTargetLocomotion()
        {
        }

        public RotationByMouseOrTargetLocomotion(Camera camera)
        {
            Camera = camera;
        }

        public Camera Camera
        {
            get => _camera;
            set => _camera = value;
        }

        public Vector3 MouseScreenPosition
        {
            get => _mouseScreenPosition;
            set => _mouseScreenPosition = value;
        }

        public IPositionTarget Target
        {
            get => _target;
            set => _target = value;
        }

        public override Context OnUpdate(Context context)
        {
            _base ??= new RotationLocomotion(); // 反序列化后懒重建
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
