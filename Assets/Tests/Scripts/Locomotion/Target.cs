using System;
using UnityEngine;
namespace Tests.Locomotion
{
    class Target : ITarget
    {
        Camera _camera;
        ThirdPersonCameraController _controller;
        internal Context context;

        public Target(Camera camera)
        {
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));
            _controller = _camera.GetComponent<ThirdPersonCameraController>() ?? throw new ComponentCantFoundException(_camera.gameObject, typeof(ThirdPersonCameraController));
        }

        public Vector3 Position
        {
            get
            {
                _controller.UpdatePos(context.Locomotion.Position);
                var ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hitInfo))
                    return hitInfo.point;
                return default;
            }
        }
    }
}