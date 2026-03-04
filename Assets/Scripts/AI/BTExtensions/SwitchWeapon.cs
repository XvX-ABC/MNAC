using System;
using MNAC.Utilities.Timeline;
using MNAC.Weapons;
using UnityEngine;
using TaskStatus = BehaviorDesigner.Runtime.Tasks.TaskStatus;

namespace Assets.Tests.Scripts.AI.BTExtensions
{
    internal class SwitchWeapon : WeaponActionBase
    {
        [SerializeField]
        string _targetWeaponName;
        string _currentWeaponName;
        ITimeline _switchTimeline;
        bool _switchExecuted;
        bool _switchSucceed;

        public override void OnStart()
        {
            base.OnStart();
            _currentWeaponName = weapon.Name;
            _switchTimeline = controller.weaponSwitchingTimeline;
            _switchTimeline.EndAction += WhenTimelineEnd;
            controller.weaponSelectionFunc = WhenSelectWeapon;
            armInput.weaponSwitch = true;
        }
        string WhenSelectWeapon(WeaponDescription[] descriptions)
        {
            var idx = Array.FindIndex(descriptions, d => d.Name == _targetWeaponName);
            if (idx == -1)
                return _currentWeaponName;
            else
            {
                _switchSucceed = true;
                return _targetWeaponName;
            }
        }
        void WhenTimelineEnd(TimelineContext _)
        {
            _switchExecuted = true;
        }
        public override TaskStatus OnUpdate()
        {
            if (!_switchExecuted)
                return TaskStatus.Running;
            return _switchSucceed ? TaskStatus.Success : TaskStatus.Failure;
        }
        public override void OnEnd()
        {
            _switchExecuted = false;
            _switchSucceed = false;
            armInput.weaponSwitch = false;
            _switchTimeline.EndAction -= WhenTimelineEnd;
            controller.weaponSelectionFunc = null;
            base.OnEnd();
        }
    }
}
