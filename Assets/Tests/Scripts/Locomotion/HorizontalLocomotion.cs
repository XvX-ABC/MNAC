using Locomotion;
using UnityEngine;
namespace Tests.Locomotion
{
    class HorizontalLocomotion : IModule
    {
        IBaseDefinitions _definition;
        public HorizontalLocomotion(IBaseDefinitions definition)
        {
            _definition = definition;
        }

        public void OnUpdate(Context context)
        {
            var direction = context.Input.HorizontalDirection;
            if (direction == Vector3.zero)
                return;
            var currentVelocity = context.Velocity;
            var ground = context.Ground;
            if (ground != null)
            {
                var normal = ground.Normal;
                direction = Vector3.ProjectOnPlane(direction, normal);
            }
            var velocity = Vector3.MoveTowards(currentVelocity, direction * _definition.Speed, _definition.AscendingSpeed);
            context.Velocity = velocity;
        }
    }
}