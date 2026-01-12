using UnityEngine;
using TaskStatus = BehaviorDesigner.Runtime.Tasks.TaskStatus;

namespace Assets.Tests.Scripts.AI.BTExtensions
{
    internal class ArmedWeaponNameIs : WeaponConditionalBase
    {
        [SerializeField]
        string _weaponName;
        public override TaskStatus OnUpdate()
        {
            return weapon.Name == _weaponName ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
