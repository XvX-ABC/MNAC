using Locomotion;
using Tests.Environment;
using TMPro.EditorUtilities;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;
using UnityEngine.Rendering.Universal;
namespace Tests.Locomotion
{
    class HorizontalLocomotion : IModule
    {
        IBaseDefinitions _definitions;
        JumpLocomotion _jump;

        public HorizontalLocomotion(IBaseDefinitions definitions, JumpLocomotion jump)
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
            if (_jump.CurrentState > JumpLocomotion.State.OnGround || ground == null)
            {
                currentVelocity = Vector3.ProjectOnPlane(currentVelocity, world.Up);
                currentSpeed = currentVelocity.magnitude;
            }

            var speed = _definitions.Speed;
            if (currentSpeed <= _definitions.Speed)
                speed = Mathf.MoveTowards(currentSpeed, _definitions.Speed, _definitions.AccelerationSpeed);
            var velocity = direction.normalized * speed - currentVelocity;
            context.Velocity += velocity;
        }
    }
}