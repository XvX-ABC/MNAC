using System;
using Locomotion;
using UnityEngine;
namespace Tests.Locomotion
{
    public class HorizontalDrag : IModule
    {
        IBaseDefines _defines;

        public HorizontalDrag(IBaseDefines defines)
        {
            _defines = defines ?? throw new ArgumentNullException(nameof(defines));
        }

        Vector3 CalculateVelocityWithDrag(Vector3 velocity, float deltaTime)
        {

            var result = velocity * (1 - deltaTime * _defines.Drag);
            result.y = velocity.y;
            return result;
        }
        public void OnUpdate(Context context)
        {
            var direction = context.Input.HorizontalDirection;
            if (direction == Vector3.zero)
                context.Velocity = CalculateVelocityWithDrag(context.Velocity, _defines.Drag);
        }
    }
}