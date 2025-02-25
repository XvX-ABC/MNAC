using Locomotion;
using System;
using UnityEngine;
using JState = Tests.Locomotion.JumpLocomotion.State;
namespace Tests.Locomotion
{
    class AirLocomotion : IModule
    {

        IBaseDefinition _definition;
        JumpLocomotion _jump;
        public AirLocomotion(IBaseDefinition definition, JumpLocomotion jumpLocomotion)
        {
            _definition = definition ?? throw new ArgumentNullException(nameof(definition));
            _jump = jumpLocomotion ?? throw new ArgumentNullException(nameof(jumpLocomotion));
        }

        Vector3 CalculateVelocity(Vector3 currentVelocity)
        {
            var result = currentVelocity;
            result.y = _definition.AscendingSpeed;
            return result;
        }
        public void OnUpdate(Context context)
        {
            var input = context.Input;
            var ground = context.Ground;
            var state = context.State;


            if (_jump.CurrentState > JState.Idle
                && _jump.CurrentState <= JState.Ascending)
                return;


            if (!input.IsAscending)
            {
                if (state == State.Ascending)
                    context.State = State.Descending;
                return;
            }


            if (_jump.CurrentState == JState.Descending)
                _jump.EndJump();


            if (state != State.Ascending)
                context.State = State.Ascending;
            context.Velocity = CalculateVelocity(context.Velocity);

        }
    }
}