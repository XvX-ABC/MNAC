using System;
using Tests.Behaviours.Arms;
using Tests.Behaviours.Arms.Animations;
using Tests.Behaviours.Arms.Weapons;
using Tests.States;
using Tests.Weapons;
namespace Tests.Characters.Arms.Weapons.Launchers
{
    internal abstract class ArmedWeaponArmBehaviourBase_MonoComponent : StateComponentNode_MonoComponent, IArmedWeaponArmBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
        }

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

        public override void TransitionBeginWhichOfPreviousState(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.TransitionBeginWhichOfPreviousState(currentTransition);
            behaviour.TransitionBeginWhichOfPreviousState(currentTransition);
        }

        public override void TransitionRunningWhichOfPreviousState(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.TransitionRunningWhichOfPreviousState(currentTransition);
            behaviour.TransitionRunningWhichOfPreviousState(currentTransition);
        }
        public override void TransitionEndWhichOfPreviousState(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.TransitionEndWhichOfPreviousState(currentTransition);
            behaviour.TransitionEndWhichOfPreviousState(currentTransition);
        }
        public override void TransitionBeginWhichToNextState(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.TransitionBeginWhichToNextState(currentTransition);
            behaviour.TransitionBeginWhichToNextState(currentTransition);
        }
        public override void TransitionRunningWhichToNextState(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.TransitionRunningWhichToNextState(currentTransition);
            behaviour.TransitionRunningWhichToNextState(currentTransition);
        }
        public override void TransitionEndWhichToNextState(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.TransitionEndWhichToNextState(currentTransition);
            behaviour.TransitionEndWhichToNextState(currentTransition);
        }
    }
}
