using System.Runtime.InteropServices;
using Tests.Behaviours.Input;
using Tests.TPhysics.Environment;
using Tests.TPhysics.Locomotion;
using UnityEngine;
using LCore = Tests.TPhysics.Locomotion.LocomotionCore;
namespace Tests.Characters.Humanoid.Locomotion
{
    internal class MutativeDragController
    {
        MutativeDrag _mdrag;
        IBaseInput _input;
        IGroundDetector _groundsDetector;
        LCore _lcore;
        LocomotionStatemachine _statemachine;
        LocomotionStatemachine _movementStatemachine;
        public MutativeDragController(float transitionDuration, Vector2 range, IGroundDetector groundsDetector, IBaseInput input, LCore lcore, LocomotionStatemachine statemachine, LocomotionStatemachine movementStatemachine)
        {
            _mdrag = new(transitionDuration, range);
            _groundsDetector = groundsDetector;
            _input = input;
            _lcore = lcore;
            _lcore.AddModule(_mdrag);
            _statemachine = statemachine;
            _movementStatemachine = movementStatemachine;
        }

        public void OnUpdate()
        {
            var direction = _input.HorizontalVector;
            if (direction != Vector3.zero || _groundsDetector.Grounds.Count == 0 || _statemachine.CurrentState != _movementStatemachine)
            {
                _lcore.DisableModule(_mdrag);
            }
            else
                _lcore.EnableModule(_mdrag);
            //if (_forceEnable || (direction == Vector3.zero && _groundsDetector.Grounds.Count > 0 && _statemachine.CurrentState == _movementStatemachine))
            //    _lcore.EnableModule(_mdrag);
            //else
            //    _lcore.DisableModule(_mdrag);
        }
    }
}
