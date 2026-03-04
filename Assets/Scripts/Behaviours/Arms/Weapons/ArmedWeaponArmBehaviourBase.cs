using System;
using System.Text;
using MNAC.States;
using MNAC.Weapons;
using MNAC.Weapons;
using WeaponType = MNAC.Weapons.WeaponType;

namespace MNAC.Behaviours.Arms.Weapons
{
    public abstract class ArmedArmBehaviourBase : IArmedArmBehaviour
    {
        protected bool enabled;
        public virtual bool Activated { get => enabled; set => enabled = value; }
        public abstract WeaponType Type { get; }
        public abstract IWeapon Weapon { get; set; }
        public abstract IArmedArmAnimationPlayablePart Animator { get; }
        public abstract Func<bool> ActivationTrigger { get; }
        public abstract Func<bool> UnactivationTrigger { get; }
        public abstract IWithCallbackPlayableState<object> State { get; }

        public virtual void BehaviourOnFixedUpdate()
        {
        }

        public virtual void BehaviourOnUpdate()
        {
        }
        public virtual void BehaviourOnLateUpdate() { }
    }
}

