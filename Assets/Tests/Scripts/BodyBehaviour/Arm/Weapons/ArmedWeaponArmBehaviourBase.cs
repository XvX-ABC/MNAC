using System;
using Tests.Characters;
using Tests.States;
using Tests.Utilities.MTrees;
using Tests.Weapons;
using UnityEngine;

namespace Tests.Behaviours.Arm.Weapons
{
    public abstract class ArmedWeaponArmBehaviourBase : PlayableStateMonoComponentBase, IArmedWeaponArmBehaviour
    {
        [SerializeField]
        protected WeaponType type;
        internal ComponentNode node;
        protected Blackboard blackboard;
        protected GameObject armObj;
        [Obsolete]
        protected internal byte statusNum;
        protected internal bool isActivated;
        public WeaponType Type { get => type; }
        public abstract IWeapon Weapon { get; set; }
        public abstract IArmedWeaponArmAnimator Animator { get; }


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
        public bool IsActivated { get => isActivated; }
        public abstract IPlayableState<object> State { get; }

        protected override void Awake()
        {
            base.Awake();
            node = new(this);
        }
        public virtual void Initialize(GameObject armObj)
        {
            this.armObj = armObj ?? throw new ArgumentNullException(nameof(armObj));
        }
        public virtual void Initialize(Blackboard blackboard)
        {
            this.blackboard = blackboard;
        }
        public virtual void Dispose()
        {
            this.blackboard = null;
        }

    }
}

