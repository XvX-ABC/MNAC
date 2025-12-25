using Tests.AI;
using Tests.Characters.Humanoid.Arms;
using Tests.Weapons_New;
using UnityEngine;
using TaskStatus = BehaviorDesigner.Runtime.Tasks.TaskStatus;

namespace Assets.Tests.Scripts.AI.BTExtensions
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
