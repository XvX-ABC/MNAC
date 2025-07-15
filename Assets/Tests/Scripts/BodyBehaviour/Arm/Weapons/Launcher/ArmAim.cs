using Assets.Scripts.Utilities.Timeline.Event.Point;
using RootMotion.FinalIK;
using System;
using System.Data;
using Tests.Behaviours.Arm;
using UnityEngine;

namespace Tests.BodyBehaviour.Arm.Weapons.Launcher
{
    internal class ArmAim : ArmedArmStateBase, IAimer
    {
        AimIK _aimIK;
        ITarget _target;
        internal Action<float> weightChangedAction;
        public ITarget Target
        {
            get => _target;
            set
            {
                if (value != null)
                    _target = value;
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
            if (_target != null)
                _aimIK.solver.SetIKPosition(_target.Position);
        }
    }
}
