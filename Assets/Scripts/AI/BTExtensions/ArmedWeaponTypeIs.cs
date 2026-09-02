using MNAC.AI;
using MNAC.Characters.Humanoid.Arms;
using MNAC.Weapons;
using UnityEngine;
using TaskStatus = BehaviorDesigner.Runtime.Tasks.TaskStatus;

namespace MNAC.AI.BTExtensions
{
    internal abstract class ArmedWeaponTypeIs<T> : AIConditionalBase where T : IWeapon
    {
        [SerializeField]
        TryGetArm _getArm;
        ArmController _controller;
        public override void OnStart()
        {
            base.OnStart();
            _controller = _getArm.controller;
        }
        public override TaskStatus OnUpdate()
        {
            return _controller.currentWeapon is T ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
