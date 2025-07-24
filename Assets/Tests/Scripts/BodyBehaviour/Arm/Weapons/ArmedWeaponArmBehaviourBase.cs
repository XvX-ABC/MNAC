using Tests.Characters;
using Tests.States;
using Tests.Utilities.MTrees;
using Tests.Weapons;
using UnityEngine;

namespace Tests.Behaviours.Arm.Weapons
{
    public abstract class ArmedWeaponArmBehaviourBase : PlayableStateUComponentBase, IArmedWeaponArmBehaviour
    {
        [SerializeField]
        protected WeaponType type;
        internal ComponentNode node;
        protected Blackboard blackboard;
        public WeaponType Type { get => type; }
        public abstract IWeapon Weapon { get; set; }
        public abstract IArmedWeaponArmAnimator Animator { get; }


        public virtual Blackboard Blackboard
        {
            get => blackboard;
            set
            {
                blackboard = value;
                node.UpdateBlackboardForChildren();
            }
        }
        public ICharacterComponentNode Node { get => node; }

        protected override void Awake()
        {
            base.Awake();
            node = new(this.ID, this);
        }
    }
}

