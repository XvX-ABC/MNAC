using System;
using Locomotion;
using UnityEngine;
namespace Tests.Locomotion
{
    public class HorizontalDrag : IModule
    {
        IBaseDefinitions _definition;
        Rigidbody _rb;

        public HorizontalDrag(IBaseDefinitions definitions, Rigidbody rbody)
        {
            _definition = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _rb = rbody ?? throw new ArgumentNullException(nameof(rbody));
        }

        Vector3 CalculateVelocityWithDrag(Vector3 velocity, float deltaTime)
        {
            var result = velocity * (1 - deltaTime * _definition.Drag);
            return result;


        }
        public void OnFixedUpdate(Context context)
        {
            var direction = context.Input.HorizontalDirection;
            var expectedSpeed = context.ExpectedLocomotion.SquareSpeed;
            var currentSpeed = _rb.velocity.sqrMagnitude;
            //if (direction == Vector3.zero)
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