using Locomotion;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Rendering.Universal;
namespace Tests.Locomotion
{
    class HorizontalLocomotion : IModule
    {
        IBaseDefinitions _definitions;
        JumpLocomotion_New _jump;

        public HorizontalLocomotion(IBaseDefinitions definitions, JumpLocomotion_New jump)
        {
            _definitions = definitions;
            _jump = jump;

        }

        public void OnFixedUpdate(Context context)
        {
            var world = context.World;
            var direction = world.Input.HorizontalDirection;
            if (direction == Vector3.zero)
                return;
            var currentVelocity = context.Velocity;
            var currentSpeed = context.Speed;


            var ground = context.Ground;
            if (_jump.CurrentState > JumpLocomotion_New.State.OnGround || ground == null)
            //if (_jump.CurrentState > JumpLocomotion_New.State.OnGround)
            {
                currentVelocity = Vector3.ProjectOnPlane(currentVelocity, world.Up);
                currentSpeed = currentVelocity.magnitude;
            }

            var speed = _definitions.Speed;
            if (currentSpeed <= _definitions.Speed)
                //var velocity = Vector3.MoveTowards(currentVelocity, direction * _definitions.Speed, _definitions.AscendingSpeed) - currentVelocity;
                speed = Mathf.MoveTowards(currentSpeed, _definitions.Speed, _definitions.AccelerationSpeed);
            var velocity = direction.normalized * speed - currentVelocity;
            context.Velocity += velocity;
        }
    }
}