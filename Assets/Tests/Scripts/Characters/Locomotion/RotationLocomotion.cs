using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tests.Input;
using Tests.Interaction;
using Tests.TPhysics.Locomotion;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.Characters.Locomotion
{
    public class RotationLocomotion : ComponentBase
    {
        TPhysics.Locomotion.LocomotionCore _core;
        Camera _camera;
        Rigidbody _rb;
        RotationOnScreenLocomotion _locomotion;

        ITarget _target;
        IInput _input;
        ITargetsCatcher _targetCather;
        public RotationLocomotion([NotNull] Camera camera, [NotNull] Rigidbody rigidbody, [NotNull] TPhysics.Locomotion.LocomotionCore core)
        {
            _core = core;
            _camera = camera;
            _rb = rigidbody;
            _locomotion = new();
            _locomotion.Camera = _camera;
            _core.AddModule(_locomotion, true);
        }

        public override string Name => "character_rotation";

        protected ITargetsCatcher targetCatcher
        {
            get => _targetCather;
            set
            {
                if (_targetCather != null)
                    _targetCather.TargetsChangedAction -= CatchTarget;

                if (value != null)
                    value.TargetsChangedAction += CatchTarget;
                _targetCather = value;
            }
        }

        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            if (!blackboard.TryReadValue<IInput>(CharacterBlackboardFields.Character_Input_Main, out _input))
                throw new Exception();
            if (blackboard.TryReadValue<FieldChangeHandler>(CharacterBlackboardFields.FieldChangeHandler, out var handler))
            {
                handler.RegisterAction<ITargetsCatcher>(CharacterBlackboardFields.TargetsCatcher, TargetCatherUpdate);
            }
        }
        public override void Dispose()
        {
            _core.RemoveModule(_locomotion);
            if (blackboard.TryReadValue<FieldChangeHandler>(CharacterBlackboardFields.FieldChangeHandler, out var handler))
            {
                handler.UnregisterAction<ITargetsCatcher>(CharacterBlackboardFields.TargetsCatcher, TargetCatherUpdate);
            }
            targetCatcher = null;
        }
        void TargetCatherUpdate(FieldEventType type, ITargetsCatcher nc, ITargetsCatcher oc)
        {
            targetCatcher = nc;
        }
        void CatchTarget(IList<ITarget> targets)
        {
            _target = targets.Count > 0 ? targets[^1] : null;
        }
        public void OnUpdate()
        {
            var cpos = _camera.transform.position;
            var bpos = _rb.position;
            var world = _locomotion.World;
            var r = Quaternion.LookRotation(Vector3.ProjectOnPlane(bpos - cpos, world.Up), world.Up);
            _locomotion.RotationOffset = r;

            _locomotion.OriginalPos = _camera.WorldToScreenPoint(bpos);
            _locomotion.TargetPos = _target == null ? _input.MousePosition : _target.Position;
        }
    }
}
