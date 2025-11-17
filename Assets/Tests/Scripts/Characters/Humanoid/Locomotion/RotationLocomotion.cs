using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tests.Behaviours.Input;
using Tests.Input;
using Tests.Interaction;
using Tests.TPhysics.Locomotion;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;
using LCore = Tests.TPhysics.Locomotion.LocomotionCore;

namespace Tests.Characters.Humanoid.Locomotion
{
    public class RotationLocomotion : ComponentBase
    {
        LCore _core;
        Rigidbody _rb;
        RotationByMouseOrTargetLocomotion _locomotion;

        Tests.Interaction.ITarget_Obsolete _target;
        IBaseInput _input;
        ITargetsCatcher _targetCather;

        public RotationLocomotion([NotNull] Camera camera, [NotNull] Rigidbody rigidbody, [NotNull] LCore core, IBaseInput input)
        {
            _core = core;
            _rb = rigidbody;
            _locomotion = new(camera);
            _core.AddModule(_locomotion, true);
            _input = input ?? throw new ArgumentNullException(nameof(input));
            enabled = false;
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
            //if (!blackboard.TryReadValue(CharacterBlackboardFields.Character_Input_Main_Obsolete, out _input_obsolete))
            //    throw new Exception();
            //blackboard.TryReadValueOrThrowException(CharacterBlackboardFields.Character_Input_Main, out _input);
            if (blackboard.TryReadValue<FieldChangeHandler>(CharacterBlackboardFields.FieldChangeHandler, out var handler) && !TryReadTargetsCatcher(blackboard))
            {
                handler.RegisterAction<ITargetsCatcher>(CharacterBlackboardFields.TargetsCatcher, TargetCatherUpdate);
            }
            enabled = true;
        }
        public override void Dispose()
        {
            _core.RemoveModule(_locomotion);
            if (blackboard.TryReadValue<FieldChangeHandler>(CharacterBlackboardFields.FieldChangeHandler, out var handler))
            {
                handler.UnregisterAction<ITargetsCatcher>(CharacterBlackboardFields.TargetsCatcher, TargetCatherUpdate);
            }
            targetsCatcher = null;
            enabled = false;
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
        void CatchTarget(IList<Tests.Interaction.ITarget_Obsolete> targets)
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
            //_locomotion.MouseScreenPosition = _input_obsolete.MousePosition;
            _locomotion.MouseScreenPosition = _input.MousePosition;
            _locomotion.Target = _target;
        }
    }
}
