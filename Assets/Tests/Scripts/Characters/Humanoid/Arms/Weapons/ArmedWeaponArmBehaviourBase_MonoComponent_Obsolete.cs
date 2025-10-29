using System;
using Tests.Behaviours.Arms.Weapons;
using Tests.Characters.Humanoid;
using Tests.States;
using Tests.Utilities.Composable;
using Tests.Weapons;
namespace Tests.Characters.Humanoid.Arms.Weapons
{
    [Obsolete]
    public abstract class ArmedWeaponArmBehaviourBase_MonoComponent_Obsolete : StateComponentNode_MonoComponent, IArmedWeaponArmBehaviour
    {

        public bool Activated { get => enabled; set => enabled = value; }

        public abstract WeaponType Type { get; }


        public abstract IArmedWeaponArmAnimationPlayablePart Animator { get; }

        public abstract Func<bool> EntryFunc { get; }

        public abstract Func<bool> ExitFunc { get; }


        public abstract IWeapon Weapon { get; set; }
        protected abstract ArmedWeaponArmBehaviourBase_Obsolete behaviour { get; }

        public HumanPart Part => throw new NotImplementedException();

        HumanPart IArmedWeaponArmBehaviour.Part { get => Part; set => throw new NotImplementedException(); }

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
