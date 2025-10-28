using System;
using Tests.Behaviours.Input;
using Tests.Input;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.UI
{
    public class UICore : ComponentBase_MonoComponent
    {
        //[SerializeField]
        //GameObject _inputObj;
        [SerializeField]
        Camera _camera;
        [Obsolete]
        IInput_Obsolete _input_obsolete;
        IBaseInput _input;

        internal new Camera camera { get => _camera; set => _camera = value ?? throw new NullReferenceException(nameof(camera)); }
        internal IInput_Obsolete input_Obsolete { get => _input_obsolete; set => _input_obsolete = value ?? throw new NullReferenceException(nameof(input_Obsolete)); }
        internal IBaseInput Input { get => _input; set => _input = value ?? throw new NullReferenceException(nameof(_input)); }

        [Obsolete]
        public void Initialize(Camera camera, IInput_Obsolete input)
        {
            this.input_Obsolete = input;
            this.camera = camera;
            Initialize(new Blackboard());
        }
        public void Initialize(Camera camera, IBaseInput input)
        {
            this.Input = input;
            this.camera = camera;
            Initialize(new Blackboard());
        }
        public override void Initialize(Blackboard blackboard)
        {
            //if (_inputObj != null)
                //input_Obsolete = _inputObj.GetComponent<IInput>();
            base.Initialize(blackboard);
            //blackboard.TryRegisterField(UIBlackboardFields.Input, input_Obsolete);
            blackboard.TryRegisterField(UIBlackboardFields.Input, _input);
            blackboard.TryRegisterField(UIBlackboardFields.Camera_Main, camera);

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var obj = child.gameObject;
                if (obj.TryGetComponent<UIComponent>(out var component))
                    node.AddChild(component.node);
            }
        }
    }
}
