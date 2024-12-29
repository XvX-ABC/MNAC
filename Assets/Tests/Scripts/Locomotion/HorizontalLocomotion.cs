using Locomotion;
using UnityEngine;
namespace Tests.Locomotion
{

    class HorizontalLocomotion : IModule
    {
        IBaseDefines _defines;

        public HorizontalLocomotion(IBaseDefines defines)
        {
            _defines = defines;
        }

        public void Update(Context context)
        {
            var direction = context.Input.HorizontalDirection;
            var currentVelocity = context.Velocity;
            var ground = context.Ground;
            var normal = ground.Normal;
            var touched = ground.Touched;
            if (touched)
                direction = Vector3.ProjectOnPlane(direction, normal);
            var velocity = Vector3.MoveTowards(currentVelocity, direction * _defines.Speed, _defines.AscendingSpeed);
            context.Velocity = velocity;
        }
    }
}