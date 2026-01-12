using System;
using Tests.AI;
using Tests.Characters.Humanoid;
using Tests.Characters.Humanoid.Arms;
using Tests.Weapons_New;
using UnityEngine;
using static Tests.AI.AIHumanoidInput;

namespace Assets.Tests.Scripts.AI.BTExtensions
{
    internal abstract class WeaponActionBase : AIActionBase
    {
        [SerializeField]
        TryGetArm _getArm;
        protected ArmController controller { get => _getArm?.controller; }
        protected HumanBodyPart part;
        protected AInput armInput;
        protected WInput weaponInput;
        protected IWeapon weapon { get => controller.currentWeapon ?? throw new NullReferenceException(nameof(weapon)); }
        public override void OnAwake()
        {
            base.OnAwake();
            part = _getArm.part;
            var humanInput = core.componentContext.Input;
            armInput = part switch
            {
                HumanBodyPart.LeftArm => humanInput.leftArmInput,
                HumanBodyPart.RightArm => humanInput.rightArmInput,
                _ => throw new Exception("The humanoid part must is LeftArm or RightArm.")
            };
            weaponInput = armInput.weaponInput;
        }
    }
}
