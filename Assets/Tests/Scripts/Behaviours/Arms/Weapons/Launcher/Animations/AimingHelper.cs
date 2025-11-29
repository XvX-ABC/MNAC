using RootMotion.FinalIK;
using System;
using Tests.Interaction;
using Tests.Utilities.Timeline;
using Tests.Utilities.Timeline.Events.Range;
using Tests.Weapons;
using Tests.Weapons_New;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launcher.Animations
{
    internal class AimingHelper
    {
        AimIK _aimIK;
        IGameObjTarget _currentTarget;
        IWeapon _controlledWeapon;
        Vector3 _preTargetPosition;
        ITimeline _targetChangeTimeline;

        public AimingHelper(AimIK aimIK, float targetChangeDuration)
        {
            _aimIK = aimIK ?? throw new ArgumentNullException(nameof(aimIK));
            _targetChangeTimeline = new Timeline_V1(targetChangeDuration);
            _targetChangeTimeline.AddRangeEvent(0, 1, ctx =>
            {
                if (!_aimIK.enabled || _aimIK.solver.IKPositionWeight <= 0)
                    return;
                var pos = Vector3.Lerp(_preTargetPosition, _currentTarget.Position, ctx.NormalizedTime);
                _aimIK.solver.SetIKPosition(pos);
            });
        }

        public IGameObjTarget Target
        {
            get => _currentTarget;
            set
            {
                var currentTarget = _currentTarget;
                var newTarget = value;
                _targetChangeTimeline.End();
                if (currentTarget != null && newTarget != null && currentTarget != newTarget)
                {
                    _preTargetPosition = currentTarget.Position;
                    _targetChangeTimeline.Restart();
                }
                _currentTarget = value;
            }
        }
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
            if (_aimIK.enabled && _aimIK.solver.IKPositionWeight > 0)
            {
                if (!_targetChangeTimeline.IsRunning && _currentTarget != null)
                    _aimIK.solver.SetIKPosition(_currentTarget.Position);
            }
            _targetChangeTimeline.OnUpdate(Time.deltaTime);
        }
    }
}
