using Locomotion;
using UnityEngine;
namespace Tests.Locomotion
{
    class HorizontalLocomotion : IModule
    {
        IBaseDefinition _definition;
        public HorizontalLocomotion(IBaseDefinition definition)
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
            var normal = ground.Normal;
            var touched = ground.Touched;
            if (touched)
                direction = Vector3.ProjectOnPlane(direction, normal);
            var velocity = Vector3.MoveTowards(currentVelocity, direction * _definition.Speed, _definition.AscendingSpeed);
            //velocity.y = currentVelocity.y;
            context.Velocity = velocity;
        }
    }
}