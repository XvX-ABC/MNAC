using System;
using System.Collections.Generic;
using Tests.Behaviours.Arms.Weapons;
using Tests.States;
using Tests.Weapons;

namespace Tests.Characters.Arms.Weapons
{
    internal class ArmedWeaponArmBehaviourControllerState : StateComponentNode, IArmedWeaponArmBehavioursController<IArmedWeaponArmBehaviour>
    {
        protected ArmedWeaponArmBehaviourController<IArmedWeaponArmBehaviour> controller;
        internal Behaviours.Arms.Animations.ArmAnimationCore animationCore;
        protected internal ArmedWeaponArmBehaviourControllerState(ArmedWeaponArmBehaviourController<IArmedWeaponArmBehaviour> controller) : base("weapon_armed_behaviour", 0)
        {
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
        }
        public ArmedWeaponArmBehaviourControllerState(WeaponCore weaponCore, IArmWeaponDefinitions definitions, params IArmedWeaponArmBehaviour[] behaviours) : this(new(weaponCore, definitions, behaviours))
        {
        }

        public Action<IWeapon, IArmedWeaponArmBehaviour> ActivatedAction { get => controller.ActivatedAction; set => controller.ActivatedAction = value; }
        public Action<IWeapon, IArmedWeaponArmBehaviour> UnactivatedAction { get => controller.UnactivatedAction; set => controller.UnactivatedAction = value; }
        public Func<bool> EntryFunc { get => controller.EntryFunc; }
        public Func<bool> ExitFunc { get => controller.ExitFunc; }

        IReadOnlyDictionary<string, IArmedWeaponArmBehaviour> IArmedWeaponArmBehavioursController<IArmedWeaponArmBehaviour>.Behaviours => controller.weaponBehavioursMapping;

        public void ActivateBehaviourBy(IWeapon weapon)
        {
            controller.ActivateBehaviourBy(weapon);
        }

        public void UnactivateBehaviourBy(IWeapon weapon)
        {
            controller.ActivateBehaviourBy(weapon);
        }

        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            foreach (var b in controller.behavioursCache)
                node.AddChild(b.Node);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (animationCore != null)
                animationCore.StatusNum = 1;
            controller.currentActivatedBehaviour?.OnEnter();
        }
        public override void OnExit()
        {
            controller.currentActivatedBehaviour?.OnExit();
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
            controller.currentActivatedBehaviour?.FromPreviousStateTransitionBegin(currentTransition);
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            controller.currentActivatedBehaviour?.FromPreviousStateTransitionRunning(currentTransition);
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            controller.currentActivatedBehaviour?.ToNextStateTransitionRunning(currentTransition);
        }
    }
}
