using System;
using Locomotion;
using UnityEngine;
namespace Tests.Locomotion
{
    public class HorizontalDrag : IModule
    {
        IBaseDefinitions _definition;

        public HorizontalDrag(IBaseDefinitions definition)
        {
            _definition = definition ?? throw new ArgumentNullException(nameof(definition));
        }

        Vector3 CalculateVelocityWithDrag(Vector3 velocity, float deltaTime)
        {
            var result = velocity * (1 - deltaTime * _definition.Drag);
            return result;


        }
        public void OnUpdate(Context context)
        {
            var direction = context.Input.HorizontalDirection;
            if (direction == Vector3.zero)
                context.Velocity = CalculateVelocityWithDrag(context.Velocity, context.DeltaTime);
        }
    }
}