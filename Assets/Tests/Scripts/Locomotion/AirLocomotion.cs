using Locomotion;
using System;
using UnityEngine;
using JState = Tests.Locomotion.JumpLocomotion_New.State;
namespace Tests.Locomotion
{
    class AirLocomotion : IModule
    {

        IBaseDefinitions _definitions;
        JumpLocomotion_New _jumpLocomotion;
        public AirLocomotion(IBaseDefinitions definition, JumpLocomotion_New jumpLocomotion)
        {
            _definitions = definition ?? throw new ArgumentNullException(nameof(definition));
            _jumpLocomotion = jumpLocomotion ?? throw new ArgumentNullException(nameof(jumpLocomotion));
        }

        public void OnUpdate(Context context)
        {
            var input = context.Input;
            var ground = context.Ground;
            if (ground != null || !input.IsAscending)
                return;


            if (_jumpLocomotion.CurrentState > JState.OnGround && _jumpLocomotion.CurrentState <= JState.Ascending)
                return;
            else if (_jumpLocomotion.CurrentState == JState.Descending)
                _jumpLocomotion.EndJump(context);

            var velocity = context.Velocity;
            var y = velocity.y;
            if (y <= _definitions.AscendingSpeed)
                velocity.y = _definitions.AscendingSpeed;
            context.Velocity = velocity;

        }
    }
}