using RootMotion.FinalIK;
using System;
using Tests.Behaviours.Arm.Weapons;
using Tests.Input;
using Tests.Interaction;
using Tests.States;
using Tests.Weapons.Launcher;
using UnityEngine;

namespace Tests.Behaviours.Arms
{
    [Obsolete]
    internal class ArmAim : ArmedArmStateBase
    {
        AimIK _aimIK;
        IInput_Obsolete _input;
        ITarget _target;
        internal Action<float> weightChangedAction;
        internal Action<ITarget> targetChangedAction;
        private ILauncher _weapon;

        public ITarget Target
        {
            get => _target;
            set
            {
                if (value != _target)
                {
                    Debug.Log("target changed: ");
                }
                _target = value;
                targetChangedAction?.Invoke(value);
            }

        }

        public IInput_Obsolete Input
        {
            get => _input;
            set => _input = value;
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
        public ILauncher Weapon
        {
            get => _weapon;
            set
            {
                _weapon = value;

                _aimIK.solver.transform = _weapon?.Obj == null ? null : _weapon.Obj.transform;
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
            if (_input.Fire)
            {
                _weapon.StartLaunch();
            }
            UpdateTarget();
        }
        void UpdateTarget()
        {
            if (_target != null && _aimIK.solver.IKPositionWeight > 0)
            {
                _aimIK.solver.SetIKPosition(_target.Position);
            }
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
