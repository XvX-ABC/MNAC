using Cinemachine.Editor;
using System;
using System.Linq;
using Tests.Characters.Animations;
using Tests.Characters.Arms;
using Tests.Characters.Legs;
using Tests.Characters.Locomotion;
using Tests.Input;
using Tests.States;
using Tests.Weapons;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using Utilities.Timeline;
using EnvironmentCore = Tests.Characters.Environment.EnvironmentCore;

namespace Tests.Characters
{

    public interface IInfluenceReceptor : ICharacterComponent
    {
        public bool TryGetValue<T>(out T value, object key = null);
        public bool TrySetValue<T>(T value, object key = null);
        public void Update();
    }
    public abstract class InfluenceReceptorBase : CharacterComponentBase, IInfluenceReceptor
    {
        public virtual new bool Enabled { get => enabled; set => enabled = value; }
        public virtual void Update()
        {
        }

        public abstract bool TryGetValue<T>(out T value, object key = null);
        public abstract bool TrySetValue<T>(T value, object key = null);
    }
    public class StunReceptor : InfluenceReceptorBase, IInfluenceReceptor
    {
        internal ITimeline timeline;
        public StunReceptor()
        {
            timeline = new Timeline_V1(0);
            timeline.EndAction += _ => base.Enabled = false;
        }
        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                if (value)
                    timeline.Restart();
                else if (timeline.IsRunning)
                    timeline.End();
                base.Enabled = value;
            }
        }
        public override string Name => "stun_receptor";

        public override bool TryGetValue<T>(out T value, object key = null)
        {
            value = default;
            if (key == null)
            {
                var r = typeof(T) == typeof(float);
                value = r ? (T)Convert.ChangeType(timeline.Length, typeof(T)) : default;
                return r;
            }
            return false;
        }

        public override bool TrySetValue<T>(T value, object key = null)
        {
            if (key == null)
            {
                if (value is float f)
                {
                    timeline.UpdateLength(f);
                    return true;
                }
            }
            return false;
        }

        public override void Update()
        {
            timeline.OnUpdate(Time.deltaTime);
        }

    }
    public class InfluenceReceivingCore : CharacterComponentBase
    {
        IInfluenceReceptor[] _receptors;

        public InfluenceReceivingCore(params IInfluenceReceptor[] receptors)
        {
            _receptors = receptors;
        }

        public override string Name => "influence_receiving_core";
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            foreach (var r in _receptors)
            {
                r?.Initialize(blackboard);
            }
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Influence_Receiving_Core, this);
        }
        public T FindReceptor<T>() where T : IInfluenceReceptor
        {
            return (T)_receptors.FirstOrDefault(r => r is T);
        }
        public IInfluenceReceptor FindReceptor(string name)
        {
            return _receptors.FirstOrDefault(r => r.Name == name);
        }
        public void Update()
        {
            foreach (var r in _receptors)
                if (r != null && r.Enabled)
                    r.Update();
        }
    }
    public class CharacterCore : CharacterComponentBase_MonoComponent, ICharacterComponent
    {
        [SerializeField]
        Camera _camera;
        [SerializeField]
        internal ArmCore leftArm;

        [SerializeField]
        internal LegsCore _legsCore;
        [SerializeField]
        WeaponCore _weaponCore;
        [SerializeField]
        CustomPlayerInput _input;
        [SerializeField]
        Target _target;


        CharacterAnimator _animator;

        LocomotionCore _locomotionCore;
        EnvironmentCore _environmentCore;

        InfluenceReceivingCore _influenceReceivingCore;


        CharacterBehavioursStatemachine _statemachine;
        CharacterBehavioursStateContext _context;

        protected override void Awake()
        {
            base.Awake();
            _environmentCore = GetComponent<EnvironmentCore>() ?? throw new ComponentCantFindException(this.gameObject, typeof(EnvironmentCore));
            _locomotionCore = GetComponent<LocomotionCore>() ?? throw new ComponentCantFindException(this.gameObject, typeof(EnvironmentCore));

            Initialize(new Blackboard());
        }
        void OnEnable()
        {
            if (_animator != null)
                _animator.Enabled = true;
        }
        void Start()
        {
            InitializeEnvironmentCore();

            InitializeAnimator();

            InitializeLocomotionCore();

            InitializeArmCore();

            InitializeInfluenceCore();

            InitializeStatemachine(_influenceReceivingCore);


            _animator.InitializeArmsAnimation();
            _animator.InitializeStatemachine();

            //InitializeLocomotionAnimator();
        }
        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.P))
            {
                var repecptor = _influenceReceivingCore.FindReceptor<StunReceptor>();
                repecptor.TrySetValue(10f);
                repecptor.Enabled = true;
            }
        }
        void FixedUpdate()
        {
            _influenceReceivingCore.Update();
            _statemachine.OnUpdate();
            _animator.Update();
            //Debug.Log(_statemachine);
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
            var stunReceptor = new StunReceptor();
            _influenceReceivingCore = new(stunReceptor);

            node.AddChild(_influenceReceivingCore.Node);
        }


        void InitializeStatemachine(InfluenceReceivingCore influenceCore)
        {
            var stunReceptor = influenceCore.FindReceptor<StunReceptor>() ?? throw new NullReferenceException("stunReceptor");

            var stunningState = new StunningState(stunReceptor.timeline);
            var normalState = new NormalState(_locomotionCore, leftArm);
            _context = new();
            _statemachine = new(_context, this.gameObject.name);
            _statemachine.AddState(normalState);
            _statemachine.AddState(stunningState);

            var l_s = new BlendingTransition<object>(normalState, stunningState, () => stunReceptor.Enabled, null, 0.5f);
            var s_l = new SubStatemachineTransition<object>(stunningState, normalState, _locomotionCore.movementStatemachine, () => !stunReceptor.Enabled, null, 0.5f, 0, 1);


            _statemachine.AddTransitionFor(l_s);
            _statemachine.AddTransitionFor(s_l);
        }

        public override void Initialize(Blackboard blackboard)
        {
            blackboard.TryRegisterField(CharacterBlackboardFields.Input, _input);
            blackboard.TryRegisterField(CharacterBlackboardFields.WeaponCore, _weaponCore);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Camera_Main, _camera);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Obj_Main, this.gameObject);


            blackboard.TryReadValue<FieldChangeHandler>(CharacterBlackboardFields.FieldChangeHandler, out var handler);

            var rbody = GetComponent<Rigidbody>() ?? throw new ComponentCantFindException(this.gameObject, typeof(Rigidbody));
            blackboard.TryRegisterField(CharacterBlackboardFields.Rigidbody, rbody);

            this.blackboard = blackboard;
        }
    }
}
