using RootMotion.FinalIK;
using System;
using Tests.Behaviours.Arms.Weapons;
using Tests.States;
using UnityEngine;

namespace Tests.BodyBehaviour.Arm.Weapons.Launcher
{
    internal class ArmAim : ArmedArmStateBase, IAimer_Obsolete
    {
        AimIK _aimIK;
        ITarget _target;
        internal Action<float> weightChangedAction;
        internal Action<ITarget> targetChangedAction;
        public ITarget Target
        {
            get => _target;
            set
            {
                _target = value;
                targetChangedAction?.Invoke(value);
            }

        }
        public float Weight
        {
            get => _aimIK.solver.IKPositionWeight;
            set
            {
                var v = Mathf.Clamp01(value);
                _aimIK.solver.IKPositionWeight = v;
                weightChangedAction?.Invoke(v);
            }
        }
        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                _aimIK.enabled = value;
            }
        }
        public ArmAim(AimIK aimIK) : base("aim", 0)
        {
            _aimIK = aimIK ?? throw new ArgumentNullException(nameof(aimIK));
        }


        public override void OnUpdate()
        {
            UpdateTarget();
        }
        void UpdateTarget()
        {
            if (_target != null && _aimIK.solver.IKPositionWeight > 0)
            {
                _aimIK.solver.SetIKPosition(_target.Position);
            }
        }
        public override void OnEnter()
        {
            //this.Weight = 1;
        }
        public override void OnExit()
        {
            //this.Weight = 0;
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            UpdateTarget();
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            UpdateTarget();
        }
    }
}
