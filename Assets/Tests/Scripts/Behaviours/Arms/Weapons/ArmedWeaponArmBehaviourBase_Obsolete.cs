using System;
using Tests.States;
using Tests.Weapons;

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

        public void FixedUpdate()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
    public abstract class ArmedWeaponArmBehaviourBase_Obsolete : PlayableStateBase, IArmedWeaponArmBehaviour
    {
        [Obsolete]
        protected internal byte statusNum;

        protected ArmedWeaponArmBehaviourBase_Obsolete(string name, float duration = 0, bool enabled = true) : base($"armed_{name}", duration, enabled)
        {
        }

        public abstract WeaponType Type { get; }
        public abstract IWeapon Weapon { get; set; }
        public abstract IArmedWeaponArmAnimationPlayablePart Animator { get; }


        public virtual bool Activated { get => base.Enabled; set => base.Enabled = value; }
        public abstract Func<bool> EntryFunc { get; }
        public abstract Func<bool> ExitFunc { get; }

        public void FixedUpdate()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}

