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
            _controller = _camera.GetComponent<ThirdPersonCameraController>() ?? throw new ComponentCantFindException(_camera.gameObject, typeof(ThirdPersonCameraController));
        }

        public Vector3 Position
        {
            get
            {
                _controller.UpdatePos(context.Position);
                var ray = _camera.ScreenPointToRay(Input.mousePosition);
                var layer = LayerMask.NameToLayer("Terrain");
                if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, (1 << layer)))
                    return hitInfo.point;
                return default;
            }
        }
    }
}