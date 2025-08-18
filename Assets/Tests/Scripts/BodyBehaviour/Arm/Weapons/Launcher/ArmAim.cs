using Assets.Scripts.Utilities.Timeline.Event.Point;
using RootMotion.FinalIK;
using System;
using System.Data;
using Tests.Behaviours.Arm;
using Tests.Characters;
using Tests.States;
using UnityEngine;
using static Tests.Behaviours.Arm.Weapons.ArmedLauncherArmBehaviour;

namespace Tests.BodyBehaviour.Arm.Weapons.Launcher
{
    internal class ArmAim : ArmedArmStateBase, IAimer
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
        public override void TransitionBeginWhichOfPreviousState(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.TransitionBeginWhichOfPreviousState(currentTransition);
        }
        public override void TransitionRunningWhichToNextState(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.TransitionRunningWhichToNextState(currentTransition);
            UpdateTarget();
        }
        public override void TransitionRunningWhichOfPreviousState(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.TransitionRunningWhichOfPreviousState(currentTransition);
            UpdateTarget();
        }
    }
}
