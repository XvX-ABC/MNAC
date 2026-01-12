using System;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.UI
{
    public class UICore : ComponentBase_MonoComponent
    {

        [SerializeField]
        Camera _camera;
        [SerializeField]
        UIComponent[] _components;
        [SerializeField]
        UICoreInput_Debug _inputCore;
        [SerializeField]
        bool _allowInitializeOnAwake;


        IInput _input;

        internal new Camera camera { get => _camera; set => _camera = value ?? throw new NullReferenceException(nameof(camera)); }
        internal IInput Input { get => _input; set => _input = value ?? throw new NullReferenceException(nameof(_input)); }
        protected override void Awake()
        {
            base.Awake();
            if (_input == null)
                _input = _inputCore;
            if (_allowInitializeOnAwake)
                Initialize(_camera, _input);
        }

        public void Initialize(Camera camera, IInput input)
        {
            this.Input = input;
            this.camera = camera;
            Initialize(new Blackboard());
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryRegisterField(UIBlackboardFields.Input, _input);
            blackboard.TryRegisterField(UIBlackboardFields.Camera_Main, camera);

            for (int i = 0; i < _components.Length; i++)
            {
                var c = _components[i];
                if (c == null)
                    continue;
                Node.AddChild(c.Node);
            }
            //for (int i = 0; i < transform.childCount; i++)
            //{
            //    var child = transform.GetChild(i);
            //    var obj = child.gameObject;
            //    if (obj.TryGetComponent<UIComponent>(out var component))
            //        Node.AddChild(component.Node);
            //}
        }
        public override void Dispose()
        {
            base.Dispose();
            blackboard.TryUnregisterField(UIBlackboardFields.Input);
            blackboard.TryUnregisterField(UIBlackboardFields.Camera_Main);
        }
    }
}
