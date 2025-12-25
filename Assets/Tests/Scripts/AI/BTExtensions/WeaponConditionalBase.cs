using BehaviorDesigner.Runtime.Tasks;
using System;
using Tests.AI;
using Tests.Characters.Humanoid;
using Tests.Characters.Humanoid.Arms;
using Tests.Weapons_New;
using UnityEngine;
using static Tests.AI.AIHumanoidInput;

namespace Assets.Tests.Scripts.AI.BTExtensions
{
    internal abstract class WeaponConditionalBase : AIConditionalBase
    {
        [SerializeField]
        TryGetArm _getArm;
        protected ArmController controller { get => _getArm?.controller; }
        protected HumanPart part;
        protected WInput input;
        protected IWeapon weapon { get => controller.currentWeapon; }

        public override void OnAwake()
        {
            base.OnAwake();
            part = _getArm.part;
            var humanInput = core.componentContext.Input;
            input = part switch
            {
                HumanPart.LeftArm => humanInput.leftArmInput.weaponInput,
                HumanPart.RightArm => humanInput.rightArmInput.weaponInput,
                _ => throw new Exception("The humanoid part must is LeftArm or RightArm.")
            };
        }
    }
}
