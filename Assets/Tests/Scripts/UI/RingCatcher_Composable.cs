using BehaviorDesigner.Runtime.Tasks;
using System;
using Tests.Input;
using Tests.Utilities.Composable;
using UnityEditorInternal;
using UnityEngine;

namespace Tests.UI
{
    [RequireComponent(typeof(RingCatcher))]
    public class RingCatcher_Composable : UIComponent
    {
        RingCatcher _catcher;

        IInput _input;
        protected override void Awake()
        {
            base.Awake();
            _catcher = GetComponent<RingCatcher>() ?? throw new NullReferenceException(nameof(_catcher));
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryReadValueOrThrowException<Camera>(UIBlackboardFields.Camera_Main, out var camera);
            blackboard.TryReadValueOrThrowException(UIBlackboardFields.Input, out _input);

            if (_input == null)
                throw new NullReferenceException(nameof(_input));
            if (camera == null)
                throw new NullReferenceException(nameof(camera));

            _catcher.Camera = camera;
            blackboard.TryWriteValue(UIBlackboardFields.Catcher_Ring, _catcher);

        }
        private void Update()
        {
            _catcher.MousePosition = _input.MousePosition;
        }
    }
}
