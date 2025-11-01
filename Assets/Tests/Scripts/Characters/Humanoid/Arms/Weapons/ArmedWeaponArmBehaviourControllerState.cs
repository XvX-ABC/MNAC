using System;
using System.Collections.Generic;
using Tests.Behaviours.Arms.Weapons;
using Tests.Characters.Humanoid;
using Tests.States;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using Tests.Weapons;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons
{
    internal class ArmedWeaponArmBehaviourControllerState : StateComponentNode, IArmedWeaponArmBehavioursController<IArmedWeaponArmBehaviour>
    {
        protected ArmedWeaponArmBehaviourController<IArmedWeaponArmBehaviour> controller;
        internal Behaviours.Arms.Animations.ArmAnimationCore animationCore;
        HumanPart _part;
        [Obsolete]
        protected internal ArmedWeaponArmBehaviourControllerState(ArmedWeaponArmBehaviourController<IArmedWeaponArmBehaviour> controller) : base("weapon_armed_behaviour", 0)
        {
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
        }
        protected internal ArmedWeaponArmBehaviourControllerState(ArmedWeaponArmBehaviourController<IArmedWeaponArmBehaviour> controller, HumanPart part) : base("weapon_armed_behaviour", 0)
        {
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _part = part;
        }
        [Obsolete]
        public ArmedWeaponArmBehaviourControllerState(WeaponCore weaponCore, IArmedWeaponArmDefinitions definitions, params IArmedWeaponArmBehaviour[] behaviours) : this(new(weaponCore, definitions, behaviours))
        {
        }
        public ArmedWeaponArmBehaviourControllerState(WeaponCore weaponCore, IArmedWeaponArmDefinitions definitions, HumanPart part, params IArmedWeaponArmBehaviour[] behaviours) : this(new(weaponCore, definitions, behaviours), part)
        {
            InitializeBehaviours(this.controller.behavioursCache);
        }
        public Action<IWeapon, IArmedWeaponArmBehaviour> ActivatedAction { get => controller.ActivatedAction; set => controller.ActivatedAction = value; }
        public Action<IWeapon, IArmedWeaponArmBehaviour> UnactivatedAction { get => controller.UnactivatedAction; set => controller.UnactivatedAction = value; }
        public Func<bool> EntryFunc { get => controller.EntryFunc; }
        public Func<bool> ExitFunc { get => controller.ExitFunc; }

        IReadOnlyDictionary<string, IArmedWeaponArmBehaviour> IArmedWeaponArmBehavioursController<IArmedWeaponArmBehaviour>.Behaviours => controller.weaponBehavioursMapping;
        void InitializeBehaviours(IArmedWeaponArmBehaviour[] behaviours)
        {
            foreach (var b in behaviours)
                b.Part = _part;
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
            controller.currentActivatedBehaviour?.ToNextStateTransitionEnd(currentTransition);
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            controller.currentActivatedBehaviour?.ToNextStateTransitionRunning(currentTransition);
        }
    }
}
