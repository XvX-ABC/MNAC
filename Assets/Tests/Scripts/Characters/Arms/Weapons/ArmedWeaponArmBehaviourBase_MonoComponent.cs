using System;
using Tests.Behaviours.Arms;
using Tests.Behaviours.Arms.Animations;
using Tests.Behaviours.Arms.Weapons;
using Tests.States;
using Tests.Weapons;
namespace Tests.Characters.Arms.Weapons.Launchers
{
    public abstract class ArmedWeaponArmBehaviourBase_MonoComponent : StateComponentNode_MonoComponent, IArmedWeaponArmBehaviour
    {

        public bool Activated { get => this.enabled; set => this.enabled = value; }

        public abstract WeaponType Type { get; }

        public abstract IPlayableState<object> StateNode { get; }

        public abstract IArmedWeaponArmAnimationPlayablePart Animator { get; }

        public abstract Func<bool> EntryFunc { get; }

        public abstract Func<bool> ExitFunc { get; }


        public abstract IWeapon Weapon { get; set; }
        protected abstract Behaviours.Arms.Weapons.ArmedWeaponArmBehaviourBase behaviour { get; }
        public override void OnEnter()
        {
            base.OnEnter();
            behaviour.OnEnter();
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            behaviour.OnUpdate();
        }
        public override void OnExit()
        {
            base.OnExit();
            behaviour.OnExit();
        }

        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            behaviour.FromPreviousStateTransitionBegin(currentTransition);
        }

        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            behaviour.FromPreviousStateTransitionRunning(currentTransition);
        }
        public override void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionEnd(currentTransition);
            behaviour.FromPreviousStateTransitionEnd(currentTransition);
        }
        public override void ToNextStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionBegin(currentTransition);
            behaviour.ToNextStateTransitionBegin(currentTransition);
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            behaviour.ToNextStateTransitionRunning(currentTransition);
        }
        public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionEnd(currentTransition);
            behaviour.ToNextStateTransitionEnd(currentTransition);
        }
    }
}
