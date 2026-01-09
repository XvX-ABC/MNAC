using System;
using System.Collections.Generic;
using Tests.Behaviours.Arms.Weapons;
using Tests.Characters.Weapons;
using Tests.States;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using Tests.Weapons_New;
using WeaponCore_Obsolete = Tests.Characters.Weapons.WeaponCore_Obsolete;


namespace Tests.Characters.Humanoid.Arms.Weapons
{
    internal class ArmedWeaponArmBehaviourControllerState : StateComponentNode, IArmedWeaponArmBehavioursController<IArmedWeaponArmBehaviour>
    {
        protected ArmedWeaponArmBehaviourController<IArmedWeaponArmBehaviour> controller;
        internal Behaviours.Arms.Animations.ArmAnimationCore animationCore;
        protected internal ArmedWeaponArmBehaviourControllerState(ArmedWeaponArmBehaviourController<IArmedWeaponArmBehaviour> controller, HumanBodyPart part) : base("weapon_armed_behaviour", 0)
        {
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
            InitializeBehaviours(this.controller.behavioursCache, part);
        }
        public Action<IWeapon, IArmedWeaponArmBehaviour> ActivatedAction { get => controller.ActivatedAction; set => controller.ActivatedAction = value; }
        public Action<IWeapon, IArmedWeaponArmBehaviour> UnactivatedAction { get => controller.UnactivatedAction; set => controller.UnactivatedAction = value; }
        public Func<bool> ActivationTrigger { get => controller.ActivationTrigger; }
        public Func<bool> UnactivationTrigger { get => controller.UnactivationTrigger; }

        IReadOnlyDictionary<string, IArmedWeaponArmBehaviour> IArmedWeaponArmBehavioursController<IArmedWeaponArmBehaviour>.Behaviours => controller.weaponBehavioursMapping;
        internal IArmedWeaponArmBehaviour currentActivatedBehaviour => controller.currentActivatedBehaviour;
        void InitializeBehaviours(IArmedWeaponArmBehaviour[] behaviours, HumanBodyPart part)
        {
            foreach (var b in behaviours)
                b.Part = part;
        }
        public void ActivateBehaviourBy(IWeapon weapon)
        {
            controller.ActivateBehaviourBy(weapon);
        }

        public void UnactivateBehaviourBy(IWeapon weapon)
        {
            controller.UnactivateBehaviourBy(weapon);
        }

        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            foreach (var b in controller.behavioursCache)
            {
                Node.AddChild(b.Node);
                b.Activated = false;
            }

        }
        public override void Dispose()
        {
            base.Dispose();
            foreach (var b in controller.behavioursCache)
            {
                Node.RemoveChild(b.Node);
            }
        }
        public override void OnEnter()
        {
            base.OnEnter();
            var b = controller?.currentActivatedBehaviour;
            if (animationCore != null)
                animationCore.StatusNum = 1;
            if (b != null)
            {
                b.OnEnter();
                if (!b.Activated)
                    b.Activated = true;
            }
        }
        public override void OnExit()
        {
            var b = controller.currentActivatedBehaviour;
            b?.OnExit();
            base.OnExit();
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            controller.currentActivatedBehaviour?.OnUpdate();
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            var b = controller.currentActivatedBehaviour;
            if (b != null)
            {
                b.Activated = true;
                b.FromPreviousStateTransitionBegin(currentTransition);
            }
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            controller.currentActivatedBehaviour?.FromPreviousStateTransitionRunning(currentTransition);
        }
        public override void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionEnd(currentTransition);
            controller.currentActivatedBehaviour?.FromPreviousStateTransitionEnd(currentTransition);
        }
        public override void ToNextStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionBegin(currentTransition);
            controller.currentActivatedBehaviour?.ToNextStateTransitionBegin(currentTransition);
        }
        public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionEnd(currentTransition);
            var b = controller?.currentActivatedBehaviour;
            if (b != null)
            {
                b.Activated = false;
                b.ToNextStateTransitionEnd(currentTransition);
            }
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            controller.currentActivatedBehaviour?.ToNextStateTransitionRunning(currentTransition);
        }
    }
}
