using System;
using Locomotion;
using Tests.Environment;
using UnityEngine;
namespace Tests.Locomotion_Obsolete
{
    public class HorizontalDrag : IModule
    {
        IBaseDefinitions _definition;

        public HorizontalDrag(IBaseDefinitions definitions)
        {
            _definition = definitions ?? throw new ArgumentNullException(nameof(definitions));
        }

        Vector3 CalculateVelocityWithDrag(Vector3 velocity, float deltaTime)
        {
            var result = velocity * (1 - deltaTime * _definition.Drag);
            return result;


        }
        public void OnFixedUpdate(Context context)
        {
            var expectedSpeed = context.ExpectedLocomotion.SquareSpeed;
            var currentSpeed = context.OriginalLocomotion.SquareSpeed;
            if (currentSpeed > expectedSpeed)
            {
                var ground = context.Ground;
                var normal = ground == null ? Vector3.up : ground.Normal;
                var currentVelocity = Vector3.ProjectOnPlane(context.Velocity, normal);

                context.Velocity += CalculateVelocityWithDrag(currentVelocity, context.DeltaTime) - currentVelocity;
            }
        }
    }
}