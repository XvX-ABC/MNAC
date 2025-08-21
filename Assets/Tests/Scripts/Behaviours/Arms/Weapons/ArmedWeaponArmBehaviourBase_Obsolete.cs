using System;
using Tests.Characters;
using Tests.States;
using Tests.Weapons;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons
{
    [Obsolete]
    public abstract class ArmedWeaponArmBehaviourBase_Obsolete : PlayableState_MonoComponent, IArmedWeaponArmBehaviour_Obsolete, Characters.Arms.IArmedWeaponArmBehaviour
    {
        [SerializeField]
        protected WeaponType type;
        internal ComponentNode node;
        protected Blackboard blackboard;
        [Obsolete]
        protected internal byte statusNum;
        public WeaponType Type { get => type; }
        public abstract IWeapon Weapon { get; set; }
        public abstract IArmedWeaponArmAnimationPlayablePart Animator { get; }


        public virtual Blackboard Blackboard
        {
            get => blackboard;
            set
            {
                blackboard = value;
            }
        }
        public ICharacterComponentNode Node { get => node; }
        [Obsolete]
        public byte StatusNum { get => statusNum; }
        public bool Activated { get => base.Enabled; set => base.Enabled = value; }
        public abstract IPlayableState<object> StateNode { get; }
        public abstract Func<bool> EntryFunc { get; set; }
        public abstract Func<bool> ExitFunc { get; set; }
        Action IWithCallbackPlayableState<object>.EntryAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Action IWithCallbackPlayableState<object>.ExitAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Action IWithCallbackPlayableState<object>.UpdateAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }



        protected override void Awake()
        {
            base.Awake();
            node = new(this);
        }
        public virtual void Initialize(Blackboard blackboard)
        {
            this.Blackboard = blackboard;
        }
        public virtual void Dispose()
        {
            this.blackboard = null;
        }

    }
}

