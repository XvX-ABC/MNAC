using System;
using Tests.Characters.Humanoid.Animations;
using Tests.Characters.Humanoid.Arms;
using Tests.Characters.Humanoid.Input;
using Tests.Characters.Humanoid.Legs;
using Tests.Characters.Humanoid.Locomotion;
using Tests.Characters.Interaction;
using Tests.Characters.MountPoints;
using Tests.Characters.UI;
using Tests.Interaction.Influence;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;
using WeaponBackpack = Tests.Characters.Weapons.WeaponBackpack;

namespace Tests.Characters.Humanoid
{
    internal class HumanoidController : CharacterComponent, IComponent
    {
        [Serializable]
        internal class RequiredComponents
        {
            [SerializeField]
            internal HumanoidInputComponent input;
            [SerializeField]
            internal UICore ui;
            [SerializeField]
            internal TargetLockerBase targetLocker;
            [SerializeField]
            internal EnvironmentCore environment;
            [SerializeField]
            internal LegsController legs;
            [SerializeField]
            internal LocomotionCore locomotion;
            [SerializeField]
            internal ArmController lefArm;
            [SerializeField]
            internal ArmController rightArm;
            [SerializeField]
            internal WeaponBackpack weaponBackpack;
            public CharacterComponent[] ToArray()
            {
                return new CharacterComponent[] { input, ui, targetLocker, environment, legs, locomotion, weaponBackpack, lefArm, rightArm };
            }
        }


        //[SerializeField]
        //Camera _camera;


        [SerializeField]
        RequiredComponents _requiredComponents;
        CharacterComponent[] _components;
        CharacterCollisionComponentsManager _collisionComponentsManager;
        [SerializeField]
        CharacterMountPointManager _mountPointManager;


        internal ArmController leftArm;
        internal ArmController rightArm;
        internal LocomotionCore locomotionCore;



        internal HumanAnimator animator;
        internal InfluenceCore influenceCore;


        CharacterBehavioursStatemachine _statemachine;
        CharacterBehavioursStateContext _context;
        internal NormalState normalState;
        internal DeathState diedState;


        Rigidbody _rbody;


        protected override void Awake()
        {
            base.Awake();

            if (leftArm != null)
                leftArm.enabled = false;
            if (rightArm != null)
                rightArm.enabled = false;
        }
        void OnEnable()
        {
            if (animator != null)
                animator.Enabled = true;
            if (_components != null)
                foreach (var comp in _components)
                {
                    if (comp == null)
                        continue;
                    comp.enabled = true;
                }

        }
        void OnDisable()
        {
            if (animator != null)
                animator.Enabled = false;
            if (_components != null)
                foreach (var comp in _components)
                {
                    if (comp == null)
                        continue;
                    comp.enabled = false;
                }
        }
        void OnDestroy()
        {
            ComponentsDispose();
            Dispose();
        }
        void InitializeComponents()
        {
            foreach (var comp in _components)
            {
                if (comp?.Node == null)
                    continue;
                if (comp is HumanoidComponent hcomp)
                    hcomp.owner = this;
                else if (comp is CharacterCollisionComponent hccomp)
                    _collisionComponentsManager.components.Add(hccomp);
                this.Node.AddChild(comp.Node);
            }
        }
        void ComponentsDispose()
        {
            _collisionComponentsManager.components.Clear();
            foreach (var comp in _components)
            {
                if (comp == null)
                    continue;
                if (comp is HumanoidComponent hcomp)
                {
                    hcomp.owner = null;
                }
                this.Node.RemoveChild(comp.Node);
            }
        }
        void InitializeAnimator()
        {

            animator = new(this);
            animator.Enabled = enabled;
            Node.AddChild(animator.Node);
        }


        public override void Initialize(Blackboard blackboard)
        {

            base.Initialize(blackboard);

            _components = _requiredComponents.ToArray();

            _collisionComponentsManager = new();



            var rbody = GetComponent<Rigidbody>() ?? throw new ComponentCantFindException(gameObject, typeof(Rigidbody));
            blackboard.TryRegisterField(CharacterBlackboardFields.Rigidbody, rbody);

            _mountPointManager.Initialize(blackboard);

            InitializeAnimator();

            InitializeComponents();

            normalState = new(locomotionCore, leftArm, rightArm);
        }

        private void OnCollisionEnter(Collision collision)
        {
            _collisionComponentsManager.OnCollisionEnterImpl(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            _collisionComponentsManager.OnCollisionExitImpl(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            _collisionComponentsManager.OnCollisionStayImpl(collision);
        }

    }
}
