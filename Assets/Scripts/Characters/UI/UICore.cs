using MNAC.Behaviours.Input;
using MNAC.Utilities.Blackboards;
using MNAC.Utilities.Composable;
using UnityEngine;
using Core = MNAC.UI.UICore;
namespace MNAC.Characters.UI
{
    [RequireComponent(typeof(Core))]
    internal class UICore : CharacterComponent
    {
        class Input : MNAC.UI.IInput
        {
            IBaseInput _input;

            public Input(IBaseInput input)
            {
                _input = input;
            }

            public Vector3 MousePosition => _input.MousePosition;
        }
        Core _core;


        protected override void Awake()
        {
            base.Awake();
            _core = GetComponent<Core>();
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryReadValueOrThrowException<IBaseInput>(CharacterBlackboardFields.Character_Input_Main_Base, out var binput);
            blackboard.TryReadValueOrThrowException<Camera>(CharacterBlackboardFields.Player_Camera_Main, out var camera);

            _core.Initialize(camera, new Input(binput));

            blackboard.TryRegisterUIBlackboard(_core.Blackboard);


        }
    }
}
