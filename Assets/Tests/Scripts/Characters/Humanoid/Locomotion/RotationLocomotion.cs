using System;
using System.Diagnostics.CodeAnalysis;
using Tests.Behaviours.Arms.Weapons.Launcher;
using Tests.Behaviours.Input;
using Tests.Interaction;
using Tests.TPhysics.Locomotion;
using Tests.Utilities;
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

        //Tests.Interaction.ITarget_Obsolete _target;
        ITargetLocker<ILockTarget> _targetLocker;
        IPositionTarget _target;
        IBaseInput _input;

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
        internal IPositionTarget target
        {
            get => _target;
            set
            {
                _locomotion.Target = value;
                _target = value;
            }
        }
        internal ITargetLocker<ILockTarget> targetLocker
        {
            get => _targetLocker;
            set
            {
                if (_targetLocker != null)
                    _targetLocker.MainTargetChangedAction -= WhenTargetChange;
                if (value != null)
                {
                    value.MainTargetChangedAction += WhenTargetChange;
                    target = value.MainLockTarget;
                }

                _targetLocker = value;
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
                handler.RegisterAction<ITargetLocker<ILockTarget>>(CharacterBlackboardFields.TargetLocker, UpdateTargetLocker);
            }
            enabled = true;
        }
        public override void Dispose()
        {
            if (blackboard.TryReadValue<FieldChangeHandler>(CharacterBlackboardFields.FieldChangeHandler, out var handler))
            {
                handler.UnregisterAction<ITargetLocker<ILockTarget>>(CharacterBlackboardFields.TargetLocker, UpdateTargetLocker);
            }
            enabled = false;
        }
        bool TryReadTargetsCatcher(Blackboard blackboard)
        {
            var r = blackboard.TryReadValue<ITargetLocker<ILockTarget>>(CharacterBlackboardFields.TargetLocker, out var targetLocker);
            this.targetLocker = targetLocker;
            return r;
        }
        void UpdateTargetLocker(FieldEventType type, ITargetLocker<ILockTarget> oc, ITargetLocker<ILockTarget> nc)
        {
            if (type == FieldEventType.Reading)
                return;
            targetLocker = nc;
        }
        void WhenTargetChange(ILockTarget _, ILockTarget target)
        {
            this.target = target;
        }
        public void OnUpdate()
        {
            if (!enabled)
                return;
            var bpos = _rb.position;
            _locomotion.Origin = bpos;
            _locomotion.MouseScreenPosition = _input.MousePosition;
        }
        ~RotationLocomotion()
        {
            _core.RemoveModule(_locomotion);
        }
    }
}
