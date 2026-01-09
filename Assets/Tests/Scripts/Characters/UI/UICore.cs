using Tests.Behaviours.Input;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;
using Core = Tests.UI.UICore;
namespace Tests.Characters.UI
{
    [RequireComponent(typeof(Core))]
    internal class UICore : CharacterComponent
    {
        static UICore s_instance;
        class Input : Tests.UI.IInput
        {
            IBaseInput _input;

            public Input(IBaseInput input)
            {
                _input = input;
            }

            public Vector3 MousePosition => _input.MousePosition;
        }
        Core _core;

        internal static UICore instance { get => s_instance; }

        protected override void Awake()
        {
            //if (s_instance != null)
            //{
            //    Destroy(this.gameObject);
            //    return;
            //}
            base.Awake();
            _core = GetComponent<Core>();
            //DontDestroyOnLoad(this.gameObject);
            //s_instance = this;
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
