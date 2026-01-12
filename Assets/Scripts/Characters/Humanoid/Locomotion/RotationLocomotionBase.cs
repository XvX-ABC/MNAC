using System;
using Tests.Interaction;
using Tests.TPhysics.Locomotion;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;
using LCore = Tests.TPhysics.Locomotion.LocomotionCore;

namespace Tests.Characters.Humanoid.Locomotion
{
    public abstract class RotationLocomotionBase : ComponentBase
    {
        LCore _lcore;
        IPositionTarget _target;
        ITargetLocker<ILockTarget> _targetLocker;

        protected RotationLocomotionBase(LCore locomotionCore)
        {
            _lcore = locomotionCore ?? throw new ArgumentNullException(nameof(locomotionCore));

        }
        ITargetLocker<ILockTarget> targetLocker
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

        internal virtual IPositionTarget target { get => _target; set => _target = value; }
        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                if (value)
                    _lcore.EnableModule(rotationLocomotionModule);
                else
                    _lcore.DisableModule(rotationLocomotionModule);

            }
        }
        protected abstract LocomotionModuleBase rotationLocomotionModule { get; }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            _lcore.AddModule(rotationLocomotionModule, true);
            if (!TryReadTargetLocker(blackboard))
                blackboard.RegisterFieldChangeAction<ITargetLocker<ILockTarget>>(CharacterBlackboardFields.Character_Components_TargetLocker, WhenTargetLockerChange);
            Enabled = true;
        }
        public override void Dispose()
        {
            Enabled = false;
            blackboard.UnregisterFieldChangeAction<ITargetLocker<ILockTarget>>(CharacterBlackboardFields.Character_Components_TargetLocker, WhenTargetLockerChange);
            _lcore.RemoveModule(rotationLocomotionModule);
            base.Dispose();
        }
        protected void WhenTargetLockerChange(FieldEventType type, ITargetLocker<ILockTarget> oc, ITargetLocker<ILockTarget> nc)
        {
            if (type == FieldEventType.Reading)
                return;
            targetLocker = nc;
        }
        protected bool TryReadTargetLocker(Blackboard blackboard)
        {
            var r = blackboard.TryReadValue<ITargetLocker<ILockTarget>>(CharacterBlackboardFields.Character_Components_TargetLocker, out var targetLocker);
            this.targetLocker = targetLocker;
            return r;
        }
        protected void WhenTargetChange(ILockTarget _, ILockTarget target)
        {
            this.target = target;
        }
        public abstract void OnUpdate();
    }
}
