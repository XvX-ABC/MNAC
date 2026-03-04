using MNAC.Characters.Weapons;
using UnityEngine;
using TaskStatus = BehaviorDesigner.Runtime.Tasks.TaskStatus;

namespace Assets.Tests.Scripts.AI.BTExtensions
{
    internal class ContainsWeaponInBackpack : WeaponConditionalBase
    {
        [SerializeField]
        string _weaponName;
        WeaponBackpack _backpack;
        public override void OnStart()
        {
            base.OnStart();
            _backpack = controller.weaponBackpack;
        }
        public override TaskStatus OnUpdate()
        {
            return _backpack.ContainsWeapon(part, _weaponName) ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
