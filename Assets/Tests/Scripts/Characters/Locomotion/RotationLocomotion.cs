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
        Rigidbody _rb;
        MouseOrTargetRotationLocomotion _locomotion;

        ITarget _target;
        IInput _input;
        ITargetsCatcher _targetCather;
        public RotationLocomotion([NotNull] Camera camera, [NotNull] Rigidbody rigidbody, [NotNull] TPhysics.Locomotion.LocomotionCore core)
        {
            _core = core;
            _rb = rigidbody;
            _locomotion = new(camera);
            _core.AddModule(_locomotion, true);
            this.enabled = false;
        }

        public override string Name => "character_rotation";

        protected ITargetsCatcher targetsCatcher
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
            if (blackboard.TryReadValue<FieldChangeHandler>(CharacterBlackboardFields.FieldChangeHandler, out var handler) && !TryReadTargetsCatcher(blackboard))
            {
                handler.RegisterAction<ITargetsCatcher>(CharacterBlackboardFields.TargetsCatcher, TargetCatherUpdate);
            }
            this.enabled = true;
        }
        public override void Dispose()
        {
            _core.RemoveModule(_locomotion);
            if (blackboard.TryReadValue<FieldChangeHandler>(CharacterBlackboardFields.FieldChangeHandler, out var handler))
            {
                handler.UnregisterAction<ITargetsCatcher>(CharacterBlackboardFields.TargetsCatcher, TargetCatherUpdate);
            }
            targetsCatcher = null;
            this.enabled = false;
        }
        bool TryReadTargetsCatcher(Blackboard blackboard)
        {
            var r = blackboard.TryReadValue<ITargetsCatcher>(CharacterBlackboardFields.TargetsCatcher, out var targetsCatcher);
            this.targetsCatcher = targetsCatcher;
            return r;
        }
        void TargetCatherUpdate(FieldEventType type, ITargetsCatcher oc, ITargetsCatcher nc)
        {
            if (type != FieldEventType.Reading)
            {
                targetsCatcher = nc;
            }
        }
        void CatchTarget(IList<ITarget> targets)
        {
            _target = targets.Count > 0 ? targets[^1] : null;
        }
        //public void OnUpdate()
        //{
        //    var cpos = _camera.transform.position;
        //    var bpos = _rb.position;
        //    var world = _locomotion.World;
        //    var r = Quaternion.LookRotation(Vector3.ProjectOnPlane(bpos - cpos, world.Up), world.Up);
        //    //_locomotion.RotationOffset = r;

        //    var bspos = _camera.WorldToViewportPoint(bpos);
        //    _locomotion.Origin = bspos;
        //    _locomotion.TargetPos = _target == null ? _camera.ScreenToViewportPoint(_input.MousePosition) : _camera.WorldToViewportPoint(_target.Position);
        //}
        public void OnUpdate()
        {
            if (!enabled)
                return;
            var bpos = _rb.position;
            _locomotion.Origin = bpos;
            _locomotion.MouseScreenPosition = _input.MousePosition;
            _locomotion.Target = _target;
        }
    }
}
