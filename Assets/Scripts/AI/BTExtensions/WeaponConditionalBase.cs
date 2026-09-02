using BehaviorDesigner.Runtime.Tasks;
using System;
using MNAC.AI;
using MNAC.Characters.Humanoid;
using MNAC.Characters.Humanoid.Arms;
using MNAC.Weapons;
using UnityEngine;
using static MNAC.AI.AIHumanoidInput;

namespace MNAC.AI.BTExtensions
{
    internal abstract class WeaponConditionalBase : AIConditionalBase
    {
        [SerializeField]
        TryGetArm _getArm;
        protected ArmController controller { get => _getArm?.controller; }
        protected HumanBodyPart part;
        protected WInput input;
        protected IWeapon weapon { get => controller.currentWeapon; }

        public override void OnAwake()
        {
            base.OnAwake();
            part = _getArm.part;
            var humanInput = core.componentContext.Input;
            input = part switch
            {
                HumanBodyPart.LeftArm => humanInput.leftArmInput.weaponInput,
                HumanBodyPart.RightArm => humanInput.rightArmInput.weaponInput,
                _ => throw new Exception("The humanoid part must is LeftArm or RightArm.")
            };
        }
    }
}
