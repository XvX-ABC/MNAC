using System;
using MNAC.AI;
using MNAC.Characters.Humanoid;
using MNAC.Characters.Humanoid.Arms;
using MNAC.Characters.Humanoid.Arms.Weapons.Sword;
using MNAC.Weapons.Sword;
using UnityEngine;
using BoostingHelper = MNAC.Characters.Humanoid.Arms.Weapons.Sword.BoostingHelper;
using SlashHelper = MNAC.Characters.Humanoid.Arms.Weapons.Sword.SlashHelper;
using TaskStatus = BehaviorDesigner.Runtime.Tasks.TaskStatus;

namespace MNAC.AI.BTExtensions
{
    internal class SwordActionBase : WeaponActionBase
    {
        protected ISword sword;
        protected ArmedSwordArmBehaviour_SO behaviour;
        protected BoostingHelper boostingHelper;
        protected SlashHelper slashHelper;
        public override void OnStart()
        {
            base.OnStart();
            sword = controller.currentWeapon as ISword ?? throw new NullReferenceException(nameof(sword));
            behaviour = controller.currentActivatedBehaviour as ArmedSwordArmBehaviour_SO ?? throw new NullReferenceException(nameof(behaviour));
            boostingHelper = behaviour.boostingHelper;
            slashHelper = behaviour.slashHelper;
        }
    }
    internal class SwordConditionalBase : WeaponConditionalBase
    {
        protected ISword sword;
        protected ArmedSwordArmBehaviour_SO behaviour;
        protected BoostingHelper boostingHelper;
        protected SlashHelper slashHelper;
        public override void OnStart()
        {
            base.OnStart();
            sword = controller.currentWeapon as ISword ?? throw new NullReferenceException(nameof(sword));
            behaviour = controller.currentActivatedBehaviour as ArmedSwordArmBehaviour_SO ?? throw new NullReferenceException(nameof(behaviour));
            boostingHelper = behaviour.boostingHelper;
            slashHelper = behaviour.slashHelper;
        }
    }
    internal class SwordBoostingColdDownCompleted : SwordConditionalBase
    {
        public override TaskStatus OnUpdate()
        {
            return boostingHelper.IsColdDowned ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
    internal class SwordAttack : SwordActionBase
    {
        bool _boostingStared;
        public override void OnStart()
        {
            base.OnStart();
            boostingHelper.state.EntryAction += WhenBoostingStart;
            weaponInput.fire = true;
        }
        public override TaskStatus OnUpdate()
        {
            if (!_boostingStared)
                return TaskStatus.Running;
            return (boostingHelper.inBoosting || slashHelper.slashing) ? TaskStatus.Running : TaskStatus.Success;
        }
        void WhenBoostingStart()
        {
            _boostingStared = true;
        }
        public override void OnEnd()
        {
            _boostingStared = false;
            weaponInput.fire = false;
            boostingHelper.state.EntryAction -= WhenBoostingStart;
            base.OnEnd();
        }
    }
    internal class TryGetArm : AIConditionalBase
    {
        [SerializeField]
        internal HumanBodyPart part;
        [HideInInspector]
        internal ArmController controller;
        public override TaskStatus OnUpdate()
        {
            var characterBlackboard = core.characterBlackboard;
            return characterBlackboard.TryReadValue(AIBlackboardFields.Character_Arm_Right_Controller, out controller) ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
