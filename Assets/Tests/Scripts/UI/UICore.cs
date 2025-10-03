using System;
using Tests.Input;
using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.UI
{
    public class UICore : ComponentBase_MonoComponent
    {
        [SerializeField]
        GameObject _inputObj;
        [SerializeField]
        Camera _camera;
        IInput _input;

        internal new Camera camera { get => _camera; set => _camera = value ?? throw new NullReferenceException(nameof(camera)); }
        internal IInput input { get => _input; set => _input = value ?? throw new NullReferenceException(nameof(input)); }

        public void Initialize(Camera camera, IInput input)
        {
            this.input = input;
            this.camera = camera;
            Initialize(new Blackboard());
        }
        public override void Initialize(Blackboard blackboard)
        {
            if (_inputObj != null)
                input = _inputObj.GetComponent<IInput>();
            base.Initialize(blackboard);
            blackboard.TryRegisterField(UIBlackboardFields.Input, input);
            blackboard.TryRegisterField(UIBlackboardFields.Camera_Main, camera);

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var obj = child.gameObject;
                Debug.Log("obj.name: " + obj.name);
                if (obj.TryGetComponent<UIComponent>(out var component))
                    node.AddChild(component.node);
            }
        }
    }
}
