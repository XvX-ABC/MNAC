using RootMotion.FinalIK;
using System;
using Tests.Interaction;
using Tests.Weapons;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launcher.Animations
{
    internal class AimingHelper
    {
        AimIK _aimIK;
        IGameObjTarget _currentTarget;
        IWeapon _controlledWeapon;
        public AimingHelper(AimIK aimIK)
        {
            _aimIK = aimIK ?? throw new ArgumentNullException(nameof(aimIK));
        }

        public IGameObjTarget Target
        public IWeapon ControlledWeapon
        {
            get => _controlledWeapon;
            set
            {
                _controlledWeapon = value;
                if (_controlledWeapon?.Obj != null)
                {
                    _aimIK.solver.transform = _controlledWeapon.Obj.transform;
                }
            }
        }
        public bool Enabled { get => _aimIK.enabled; set => _aimIK.enabled = value; }
        public float Weight
        {
            get => _aimIK.solver.IKPositionWeight;
            set
            {
                var v = Mathf.Clamp01(value);
                _aimIK.solver.IKPositionWeight = v;
            }
        }
        public void TargetUpdate()
        {
            if (_aimIK.enabled && _target != null && _aimIK.solver.IKPositionWeight > 0)
            {
                _aimIK.solver.SetIKPosition(_target.Position);
            }
        }
    }
}
