using System;
using Tests.Characters.Humanoid.Animations;
using Tests.Characters.Humanoid.Arms;
using Tests.Characters.Humanoid.Interaction.Input;
using Tests.Characters.Humanoid.Legs;
using Tests.Characters.Humanoid.Locomotion;
using Tests.Characters.Interaction;
using Tests.Characters.MountPoints;
using Tests.Characters.UI;
using Tests.Characters.Weapons;
using Tests.Input;
using Tests.Interaction.Influence;
using Tests.Player;
using Tests.States;
using Tests.Utilities.Assets_New;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using Tests.Weapons;
using Tests.Weapons_New;
using UnityEngine;
using Health = Tests.Interaction.Influence.Health;
using Stun = Tests.Interaction.Influence.Stun;
using WeaponsCore = Tests.Characters.Weapons.WeaponCore;

namespace Tests.Characters.Humanoid
{
    internal class HumanoidController : CharacterComponent, IComponent
    {
        [Serializable]
        internal class RequiredComponents
        {
            [SerializeField]
            internal HumanInput_MonoComponent input;
            [SerializeField]
            internal UICore ui;
            [SerializeField]
            internal TargetLocker targetLocker;
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
            internal WeaponsCore weapons;
            public CharacterComponent[] ToArray()
            {
                return new CharacterComponent[] { input, ui, targetLocker, environment, legs, locomotion, weapons, lefArm, rightArm };
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


        [SerializeField]
        ResourceLoader<TargetLocker> _targetLockerLoader;



        internal HumanAnimator animator;
        internal InfluenceCore influenceCore;


        CharacterBehavioursStatemachine _statemachine;
        CharacterBehavioursStateContext _context;
        internal NormalState normalState;
        internal DiedState diedState;

        protected override void Awake()
        {
            base.Awake();



            _components = _requiredComponents.ToArray();

            _collisionComponentsManager = new();


            //InitializeInfluenceCore();


            //Initialize(new Blackboard());


            if (leftArm != null)
                leftArm.enabled = false;
            if (rightArm != null)
                rightArm.enabled = false;
        }
        void OnEnable()
        {
            if (animator != null)
                animator.Enabled = true;

        }
        void Start()
        {

            ////InitializeUI();

            //InitializeAnimator();

            ////InitializeTargetLocker();

            //InitializeComponents();



            ////InitializeArmController();



            ////InitializeStatemachine(influenceCore);


            //_animator.InitializeArmsAnimation();
            //_animator.InitializeStatemachine();

        }
        void FixedUpdate()
        {
            //influenceCore.Update();
            //_statemachine.OnUpdate();

            animator.Update();
        }
        void OnDisable()
        {
            if (animator != null)
                animator.Enabled = false;
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
                if (comp == null)
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

            //blackboard.TryRegisterField(CharacterBlackboardFields.Player_Camera_Main, _camera);
            //blackboard.TryRegisterField(CharacterBlackboardFields.Character_Obj_Main, gameObject);

            //blackboard.TryRegisterField(CharacterBlackboardFields.Character_Influence_Core, influenceCore);



            var rbody = GetComponent<Rigidbody>() ?? throw new ComponentCantFindException(gameObject, typeof(Rigidbody));
            blackboard.TryRegisterField(CharacterBlackboardFields.Rigidbody, rbody);

            _mountPointManager.Initialize(blackboard);


            //InitializeUI();

            InitializeAnimator();

            //InitializeTargetLocker();

            InitializeComponents();



            //InitializeArmController();



            //InitializeStatemachine(influenceCore);


            //animator.InitializeArmsAnimation();
            //animator.InitializeNormalState();
            //animator.InitializeStatemachine();


            normalState = new(locomotionCore, leftArm, rightArm);


            //this.blackboard = blackboard;
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
