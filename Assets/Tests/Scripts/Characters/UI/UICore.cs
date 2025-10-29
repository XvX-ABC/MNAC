using BehaviorDesigner.Runtime.Tasks;
using Tests.Behaviours.Input;
using Tests.Input;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;
using Core = Tests.UI.UICore;
namespace Tests.Characters.UI
{
    [RequireComponent(typeof(Core))]
    public class UICore : ComponentBase_MonoComponent
    {
        Core _core;
        protected override void Awake()
        {
            base.Awake();
            _core = GetComponent<Core>();
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            //blackboard.TryReadValueOrThrowException<IInput_Obsolete>(CharacterBlackboardFields.Character_Input_Main_Obsolete, out var input);
            blackboard.TryReadValueOrThrowException<IBaseInput>(CharacterBlackboardFields.Character_Input_Main_Base, out var binput);
            blackboard.TryReadValueOrThrowException<Camera>(CharacterBlackboardFields.Character_Camera_Main, out var camera);

            //_core.Initialize(camera, input);
            _core.Initialize(camera, binput);

            //blackboard.TryRegisterField(CharacterUIBlackboardFields.Blackboard_Main, _core.Blackboard);
            blackboard.TryRegisterUIBlackboard(_core.Blackboard);


        }
    }
}
