using System;
using MNAC.Behaviours.Arms.Weapons;
using MNAC.States;
using MNAC.Utilities.Composable;
using MNAC.Weapons;
using MNAC.Weapons;
using UnityEngine;
using WeaponType = MNAC.Weapons.WeaponType;

namespace MNAC.Characters.Humanoid.Arms.Weapons
{
    public abstract class ArmedArmBehaviourBase_SO : StateComponentNode_SO, IArmedArmBehaviour
    {
        HumanBodyPart _part;
        protected bool enabled;
        protected abstract Behaviours.Arms.IArmedArmBehaviour behaviour { get; }
        public virtual bool Activated
        {
            get => enabled;
            set
            {
                Debug.Log(this.GetType().Name + ", activated state changeto  : " + value);
                enabled = value;
                behaviour.Activated = value;
            }
        }

        public abstract WeaponType Type { get; }
        public virtual IWeapon Weapon { get => behaviour.Weapon; set => behaviour.Weapon = value; }
        public virtual IArmedArmAnimationPlayablePart Animator { get => behaviour.Animator; }
        public virtual Func<bool> ActivationTrigger { get => behaviour.ActivationTrigger; }
        public virtual Func<bool> UnactivationTrigger { get => behaviour.UnactivationTrigger; }
        public HumanBodyPart Part
        {
            get
            {
                if (_part != HumanBodyPart.LeftArm && _part != HumanBodyPart.RightArm)
                    throw new ArgumentException("ArmedArmBehaviourBase_MonoComponent can only be attached to LeftArm or RightArm");
                return _part;
            }
            set => _part = value;
        }
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

        public virtual void BehaviourOnUpdate()
        {
        }

        public virtual void BehaviourOnLateUpdate()
        {

        }
        public virtual void BehaviourOnFixedUpdate()
        {
        }
    }
}
