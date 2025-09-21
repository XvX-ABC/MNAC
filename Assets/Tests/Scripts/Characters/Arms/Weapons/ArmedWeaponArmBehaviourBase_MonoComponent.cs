using System;
using Tests.Behaviours.Arms.Weapons;
using Tests.States;
using Tests.Weapons;
using UnityEngine;

namespace Tests.Characters.Arms.Weapons.Launchers
{
    public abstract class ArmedWeaponArmBehaviourBase_MonoComponent : StateComponentNode_MonoComponent, IArmedWeaponArmBehaviour
    {
        protected abstract Behaviours.Arms.IArmedWeaponArmBehaviour behaviour { get; }
        public bool Activated
        {
            get => enabled;
            set
            {
                enabled = value;
                behaviour.Activated = value;
            }
        }

        public abstract WeaponType Type { get; }
        public abstract IWeapon Weapon { get; set; }
        public abstract IArmedWeaponArmAnimationPlayablePart Animator { get; }
        public abstract Func<bool> EntryFunc { get; }
        public abstract Func<bool> ExitFunc { get; }

        public override void OnEnter()
        {
            base.OnEnter();
            behaviour?.State?.OnEnter();
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            behaviour?.State?.OnUpdate();
        }
        public override void OnExit()
        {
            base.OnExit();
            behaviour?.State?.OnExit();
        }

        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            behaviour?.State?.FromPreviousStateTransitionBegin(currentTransition);
        }

        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            behaviour?.State?.FromPreviousStateTransitionRunning(currentTransition);
        }
        public override void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionEnd(currentTransition);
            behaviour?.State?.FromPreviousStateTransitionEnd(currentTransition);
        }
        public override void ToNextStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionBegin(currentTransition);
            behaviour?.State?.ToNextStateTransitionBegin(currentTransition);
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            behaviour?.State?.ToNextStateTransitionRunning(currentTransition);
        }
        public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionEnd(currentTransition);
            behaviour?.State?.ToNextStateTransitionEnd(currentTransition);
        }
    }
}
