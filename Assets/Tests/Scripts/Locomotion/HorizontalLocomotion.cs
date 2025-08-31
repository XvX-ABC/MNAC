using Locomotion;
using Tests.Environment;
using UnityEngine;
namespace Tests.Locomotion
{
    class HorizontalLocomotion : IModule
    {
        IBaseDefinitions _definitions;

        public HorizontalLocomotion(IBaseDefinitions definitions, JumpLocomotion jump)
        {
            _definitions = definitions;
            _jump = jump;

        }

        JumpLocomotion _jump;
        public void OnFixedUpdate(Context context)
        {
            var world = context.World;
            var direction = world.Input.HorizontalVector;
            if (direction == Vector3.zero)
                return;
            var currentVelocity = context.Velocity;
            var currentSpeed = context.Speed;


            var ground = context.Ground;
            if (_jump.CurrentState > JumpLocomotion.State.OnGround || ground == null)
            {
                currentVelocity = Vector3.ProjectOnPlane(currentVelocity, world.Up);
                currentSpeed = currentVelocity.magnitude;
            }

            var speed = _definitions.MaxSpeed;
            if (currentSpeed <= _definitions.MaxSpeed)
                speed = Mathf.MoveTowards(currentSpeed, _definitions.MaxSpeed, _definitions.AccelerationSpeed);
            var velocity = direction.normalized * speed - currentVelocity;
            context.Velocity += velocity;
        }
    }
}