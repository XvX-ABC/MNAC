using System;
using Tests.Characters.Humanoid.Animations;
using Tests.Characters.Humanoid.Arms;
using Tests.Characters.Humanoid.Interaction.Input;
using Tests.Characters.Humanoid.Legs;
using Tests.Characters.Humanoid.Locomotion;
using Tests.Characters.MountPoints;
using Tests.Characters.UI;
using Tests.Input;
using Tests.Interaction.Influence;
using Tests.States;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using Tests.Weapons;
using UnityEngine;
using EnvironmentCore_Composable = Tests.TPhysics.Environment.EnvironmentCore_Composable;
using Stun = Tests.Interaction.Influence.Stun;

namespace Tests.Characters.Humanoid
{
    public class HumanCore : ComponentBase_MonoComponent, IComponent
    {
        [SerializeField]
        Camera _camera;
        [SerializeField]
        CharacterMountPointManager _mountPointManager;
        [SerializeField]
        internal ArmCore leftArm;

        [SerializeField]
        internal LegsCore _legsCore;
        [SerializeField]
        WeaponCore _weaponCore;
        [Obsolete]
        CustomPlayerInput _input_obsolete;
        [SerializeField]
        HumanInput_MonoComponent _input_mc;
        HumanInput _input => _input_mc;
        [SerializeField]
        UICore _uiCore;

        ICharacterDefinitions _definitions;


        HumanAnimator _animator;

        LocomotionCore _locomotionCore;
        EnvironmentCore_Composable _environmentCore;

        internal InfluenceCore influenceCore;


        CharacterBehavioursStatemachine _statemachine;
        CharacterBehavioursStateContext _context;
        internal DiedState diedState;

        protected override void Awake()
        {
            base.Awake();
            _definitions = GetComponent<ICharacterDefinitions>() ?? throw new ComponentCantFindException(gameObject, typeof(ICharacterDefinitions));
            _environmentCore = GetComponent<EnvironmentCore_Composable>() ?? throw new ComponentCantFindException(gameObject, typeof(EnvironmentCore_Composable));
            _locomotionCore = GetComponent<LocomotionCore>() ?? throw new ComponentCantFindException(gameObject, typeof(EnvironmentCore_Composable));
            InitializeInfluenceCore();

            Initialize(new Blackboard());


        }
        void OnEnable()
        {
            if (_animator != null)
                _animator.Enabled = true;

        }
        void Start()
        {
            InitializeUI();

            InitializeEnvironmentCore();

            InitializeAnimator();

            InitializeLocomotionCore();

            InitializeArmCore();



            InitializeStatemachine(influenceCore);


            _animator.InitializeArmsAnimation();
            _animator.InitializeStatemachine();

        }
        void FixedUpdate()
        {
            influenceCore.Update();
            _statemachine.OnUpdate();
            _animator.Update();
        }
        void OnDisable()
        {
            if (_animator != null)
                _animator.Enabled = false;
        }
        void OnDestroy()
        {
            if (_animator != null)
                _animator.Dispose();
            Dispose();
        }
        void InitializeAnimator()
        {

            _animator = new(this);
            _animator.Enabled = enabled;
            Node.AddChild(_animator.Node);
        }

        void InitializeArmCore()
        {
            Node.AddChild(leftArm.Node);
        }
        void InitializeEnvironmentCore()
        {
            Node.AddChild(_environmentCore.Node);
        }
        void InitializeLocomotionCore()
        {
            Node.AddChild(_legsCore.Node);
            _legsCore.Weight = 1;

            Node.AddChild(_locomotionCore.Node);


        }
        void InitializeInfluenceCore()
        {
            var stun = new Stun();
            var health = new Health();
            influenceCore = new(stun, health);
        }
        void InitializeUI()
        {
            Node.AddChild(_uiCore.Node);
        }

        void InitializeStatemachine(InfluenceCore influenceCore)
        {
            var stun = influenceCore.FindInfluence<Stun>();
            var health = influenceCore.FindInfluence<Health>();

            var stunningState = new StunningState(stun.timeline);
            var normalState = new NormalState(_locomotionCore, leftArm);
            diedState = new DiedState(gameObject, obj => { Destroy(obj); Debug.Log("Destory"); }, _definitions.DeathDurationTime);
            _context = new();
            _statemachine = new(_context, gameObject.name);
            _statemachine.AddState(normalState);
            _statemachine.AddState(stunningState);
            _statemachine.AddState(diedState);


            {
                var l_s = new BlendingTransition<object>(normalState, stunningState, () => stun.Enabled, null, 0.5f);
                var l_d = new BlendingTransition<object>(normalState, diedState, () => !health.IsAlive, null, 0.25f);
                _statemachine.AddTransitionFor(l_s);
                //_statemachine.AddTransitionFor(l_d);
            }

            {

                var s_l = new SubStatemachineTransition<object>(stunningState, normalState, _locomotionCore.movementStatemachine, () => !stun.Enabled, null, 0.5f, 0, 1);
                var s_d = new BlendingTransition<object>(stunningState, diedState, () => !health.IsAlive, null, 0.25f);
                _statemachine.AddTransitionFor(s_l);
            }
        }

        public override void Initialize(Blackboard blackboard)
        {
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Input_Main_Obsolete, _input_obsolete);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Input_Main_Base, _input.BaseInput);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Input_Main, _input);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Weapon_Core, _weaponCore);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Camera_Main, _camera);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Obj_Main, gameObject);

            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Influence_Core, influenceCore);

            var rbody = GetComponent<Rigidbody>() ?? throw new ComponentCantFindException(gameObject, typeof(Rigidbody));
            blackboard.TryRegisterField(CharacterBlackboardFields.Rigidbody, rbody);

            _mountPointManager.Initialize(blackboard);

            this.blackboard = blackboard;
        }
    }
}
