using System;
using System.Text;
using Tests.States;
using Tests.Weapons;
using Tests.Weapons_New;
using WeaponType = Tests.Weapons_New.WeaponType;

namespace Tests.Behaviours.Arms.Weapons
{
    public abstract class ArmedWeaponArmBehaviourBase : IArmedWeaponArmBehaviour
    {
        protected bool enabled;
        public virtual bool Activated { get => enabled; set => enabled = value; }
        public abstract WeaponType Type { get; }
        public abstract IWeapon Weapon { get; set; }
        public abstract IArmedWeaponArmAnimationPlayablePart Animator { get; }
        public abstract Func<bool> EntryFunc { get; }
        public abstract Func<bool> ExitFunc { get; }
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

