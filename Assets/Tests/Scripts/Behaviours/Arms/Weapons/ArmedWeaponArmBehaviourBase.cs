using System;
using Tests.Characters;
using Tests.States;
using Tests.Weapons;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons
{
    public abstract class ArmedWeaponArmBehaviourBase : PlayableStateBase, IArmedWeaponArmBehaviour
    {
        [Obsolete]
        protected internal byte statusNum;

        protected ArmedWeaponArmBehaviourBase(string name, float duration = 0, bool enabled = true) : base($"armed_{name}", duration, enabled)
        {
        }

        public abstract WeaponType Type { get; }
        public abstract IWeapon Weapon { get; set; }
        public abstract IArmedWeaponArmAnimationPlayablePart Animator { get; }


        [Obsolete]
        public byte StatusNum { get => statusNum; }
        public virtual bool Activated { get => base.Enabled; set => base.Enabled = value; }
        [Obsolete]
        public IPlayableState<object> StateNode { get => throw new NotImplementedException(); }
        public abstract Func<bool> EntryFunc { get; }
        public abstract Func<bool> ExitFunc { get; }


    }
}

