using Tests.Characters.Animations;
using Tests.Characters.Arms;
using Tests.Characters.Legs;
using Tests.Characters.Locomotion;
using Tests.Characters.MountPoints;
using Tests.Characters.UI;
using Tests.Input;
using Tests.Interaction.Influence;
using Tests.States;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using Tests.Weapons;
using UnityEngine;
using EnvironmentCore = Tests.Characters.Environment.EnvironmentCore;
using Stun = Tests.Interaction.Influence.Stun;

namespace Tests.Characters
{
    public class CharacterCore : ComponentBase_MonoComponent, IComponent
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
        [SerializeField]
        CustomPlayerInput _input;
        [SerializeField]
        UICore _uiCore;

        ICharacterDefinitions _definitions;


        CharacterAnimator _animator;

        LocomotionCore _locomotionCore;
        EnvironmentCore _environmentCore;

        internal InfluenceCore influenceCore;


        CharacterBehavioursStatemachine _statemachine;
        CharacterBehavioursStateContext _context;
        internal DiedState diedState;

        protected override void Awake()
        {
            base.Awake();
            _definitions = GetComponent<ICharacterDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ICharacterDefinitions));
            _environmentCore = GetComponent<EnvironmentCore>() ?? throw new ComponentCantFindException(this.gameObject, typeof(EnvironmentCore));
            _locomotionCore = GetComponent<LocomotionCore>() ?? throw new ComponentCantFindException(this.gameObject, typeof(EnvironmentCore));
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
            this.Dispose();
        }
        void InitializeAnimator()
        {

            _animator = new(this);
            _animator.Enabled = enabled;
            this.node.AddChild(_animator.node);
        }

        void InitializeArmCore()
        {
            node.AddChild(leftArm.Node);
        }
        void InitializeEnvironmentCore()
        {
            node.AddChild(_environmentCore.Node);
        }
        void InitializeLocomotionCore()
        {
            this.node.AddChild(_legsCore.node);
            _legsCore.Weight = 1;

            node.AddChild(_locomotionCore.Node);


        }
        void InitializeInfluenceCore()
        {
            var stun = new Stun();
            var health = new Health();
            influenceCore = new(stun, health);
        }
        void InitializeUI()
        {
            this.node.AddChild(_uiCore.node);
        }

        void InitializeStatemachine(InfluenceCore influenceCore)
        {
            var stun = influenceCore.FindInfluence<Stun>();
            var health = influenceCore.FindInfluence<Health>();

            var stunningState = new StunningState(stun.timeline);
            var normalState = new NormalState(_locomotionCore, leftArm);
            diedState = new DiedState(gameObject, obj => { Destroy(obj); Debug.Log("Destory"); }, _definitions.DeathDurationTime);
            _context = new();
            _statemachine = new(_context, this.gameObject.name);
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
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Input_Main, _input);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Weapon_Core, _weaponCore);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Camera_Main, _camera);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Obj_Main, this.gameObject);

            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Influence_Core, influenceCore);

            var rbody = GetComponent<Rigidbody>() ?? throw new ComponentCantFindException(this.gameObject, typeof(Rigidbody));
            blackboard.TryRegisterField(CharacterBlackboardFields.Rigidbody, rbody);

            _mountPointManager.Initialize(blackboard);

            this.blackboard = blackboard;
        }
    }
}
