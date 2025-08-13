using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event.Range;
using Assets.Tests.Scripts.BodyBehaviour.Arm.Animations;
using Assets.Tests.Scripts.Weapons;
using System;
using System.Collections.Generic;
using Tests.Behaviours.Arm.Weapons;
using Tests.BodyBehaviour.Arm.Animations;
using Tests.Character;
using Tests.Characters;
using Tests.Input;
using Tests.States;
using Tests.Weapons;
using UnityEngine;
using UnityEngine.Playables;

namespace Tests.Behaviours.Arm
{
    public interface IAnimationPlayableState : IWithCallbackPlayableState<object>
    {
        public IAnimationPlayablePartNode Node { get; }
    }
    internal class AnimationTransition : BlendingTransition<object>
    {
        internal ArmAnimationCore_New animationCore;
        public AnimationTransition(
            IWithCallbackPlayableState<object> sourceState,
            IAnimationPlayableState destinationState,
            Func<bool> triggerEvent,
            Action<IPlayableState<object>, IAnimationPlayableState, float> durationEvent,
            float duration,
            float offset = 0,
            float fixedExitTime = FIXED_EXIT_TIME_INVALID_VALUE,
            byte interruptionSource = INTERRUPTION_SOURCE_DEFAULT_CODE) : base(sourceState, destinationState, triggerEvent, null, duration, offset, interruptionSource)
        {
            if (durationEvent != null)
            {
                timeline.AddRangeEvent(0, 1, ctx =>
                {
                    durationEvent.Invoke(sourceState, destinationState, ctx.NormalizedTime);
                });
            }
        }
        void StateCheck(IAnimationPlayableState state)
        {
            if (state.Node == null)
                throw new NullReferenceException("state.Node");
            if (state.Node.Value == null)
                throw new NullReferenceException("state.Node.Value");
        }
        protected override void Begin(TimelineContext ctx)
        {
            Debug.Log("this.length； " + this.timeline.Length);
            base.Begin(ctx);
            if (animationCore == null)
                return;
            animationCore.StatusNum = 2;
            var state = (IAnimationPlayableState)destinationState;
            StateCheck(state);
            var p = state.Node.Value.PlayablePart;
            if (!p.IsNull())
            {
                p.SetTime(offset);
                p.Play();
            }
        }
    }

    [RequireComponent(typeof(ArmBehaviourDefinitions))]
    [RequireComponent(typeof(ArmAnimationDefinitions))]
    public class ArmCore : StateMonoComponentBase, IArmBehaviour, IState, IDynamicPlayablePart
    {
        internal class IdleState : PlayableStateBase
        {
            ArmAnimationCore_New _core;
            internal ArmAnimationCore_New animationCore { set => _core = value; }
            public IdleState() : base("idle")
            {
            }

            public override void OnEnter()
            {
                _core.StatusNum = 3;
            }

            public override void OnExit()
            {

            }

            public override void OnUpdate()
            {
            }
        }

        public static implicit operator StateBase<object>(ArmCore core)
        {
            return core._stateMachine;
        }

        WeaponCore _weaponCore;
        IInput _input;
        [SerializeField]
        MountPoint[] _mountPoints;

        internal IArmDefinitions definitions;
        internal IArmAnimationDefinitions animationDefinitions;


        internal ArmWeaponSwitching weaponSwitching;
        internal ArmedWeaponArmBehavioursController armedWeaponController;
        internal IdleState idle;
        AnimationTransition transition_ats;

        //internal ArmAnimationCore animationCore;
        internal ArmAnimationCore_New acore_new;
        Blackboard _blackboard;
        ComponentNode _node;
        PlayableStateMachine _stateMachine;
        public Blackboard Blackboard
        {
            get => _blackboard;
            set
            {
                if (value != null)
                {
                    value.TryReadValue(CharacterBlackboardFields.Input, out _input);
                    value.TryReadValue(CharacterBlackboardFields.WeaponCore, out _weaponCore);
                }
                _blackboard = value;
            }
        }
        public ICharacterComponentNode Node
        {
            get => _node;
        }

        public IOutputSetting OutputSetting { get => acore_new.OutputSetting; set => acore_new.OutputSetting = value; }
        public Action<Playable> UpdateAction { get => throw new Exception(); set => throw new Exception(); }
        protected override void Awake()
        {
            base.Awake();
            definitions = GetComponent<ArmBehaviourDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ArmBehaviourDefinitions));
            animationDefinitions = GetComponent<ArmAnimationDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IArmAnimationDefinitions));
            _node = new(this);
        }
        void OnEnable()
        {
            this.OnEnter();
        }
        void OnDisable()
        {
            this.OnExit();
        }
        private void Start()
        {
            var mp = FindMountPoint(definitions.Weapon.MountPointName);
            var n = definitions.Weapon.Origins[0].Name;

        }
        public MountPoint FindMountPoint(string name)
        {
            foreach (var m in _mountPoints)
            {
                if (m.Name == name)
                    return m;
            }
            return null;
        }
        void InitializeChildNodes()
        {
            _node.AddChild(weaponSwitching.Node);
            _node.AddChild(armedWeaponController.Node);
        }
        public void Initialize(Blackboard blackboard)
        {
            this.Blackboard = blackboard;
            var weaponDefinitions = definitions.Weapon;
            var weaponMountPoint = FindMountPoint(weaponDefinitions.MountPointName) ?? throw new CantFindMountPointByNameException(weaponDefinitions.MountPointName);


            InitializeSwitchingBehaviour(weaponDefinitions, weaponMountPoint);

            InitializeArmedWeaponBehaviours(weaponDefinitions);



            InitializeChildNodes();

            //this.animationCore = new(this);
            this.acore_new = new(this);
            InitializeStateMachine();

            weaponSwitching.AnimationCore = acore_new;
            armedWeaponController.AnimationCore = acore_new;
            idle.animationCore = acore_new;
            transition_ats.animationCore = acore_new;

            SetDefaultWeapon(weaponMountPoint, weaponDefinitions.Origins[0].Name, _weaponCore);

        }
        void InitializeSwitchingBehaviour(IArmWeaponDefinitions definitions, MountPoint mountPoint)
        {
            weaponSwitching = new ArmWeaponSwitching(definitions, mountPoint, _weaponCore);
            weaponSwitching.ExitWhenEnd = true;
        }
        void InitializeArmedWeaponBehaviours(IArmWeaponDefinitions definitions)
        {
            var blist = new List<(string, IArmedWeaponArmBehaviour)>();
            var behaviours = this.GetComponents<IArmedWeaponArmBehaviour>();
            armedWeaponController = new ArmedWeaponArmBehavioursController(_weaponCore, definitions, behaviours);


            weaponSwitching.SwitchingEvent += (ow, nw) =>
            {
                if (ow != null)
                {
                    armedWeaponController.UnactivateBehaviourBy(ow);
                }
                if (nw != null)
                {
                    armedWeaponController.ActivateBehaviourBy(nw);
                }

                return nw;
            };
        }
        void InitializeStateMachine()
        {
            idle = new IdleState();
            _stateMachine = new(this.name);
            _stateMachine.AddState(idle);
            _stateMachine.AddState(armedWeaponController);
            _stateMachine.AddState(weaponSwitching);

            var length = definitions.Weapon.SwitchingToBehavioursDurationTime;

            transition_ats = new AnimationTransition(armedWeaponController, weaponSwitching, () => _input.Supply, (s, d, t) =>
            {
                //acore_new.StatusNum = 2;
                //if (t >= 0.3f)
                //{
                //    acore_new.ReplaySwitching();
                //}
                acore_new.SwitchingWeight = t;
            }, 1.5f, -1, 0);

            var transition_sta = new BlendingTransition<object>(weaponSwitching, armedWeaponController, () => armedWeaponController.Enabled, (s, d, t) =>
            {
                acore_new.StatusNum = 2;
                //acore_new.PauseSwitching();
                acore_new.SwitchingWeight = 1 - t;
            }, length, 0, 1, 0);
            _stateMachine.AddTransitionFor(idle, weaponSwitching, () => _input.Supply);
            _stateMachine.AddTransitionFor(idle, armedWeaponController, 0, () => armedWeaponController.Enabled, (_, _, _) => { acore_new.StatusNum = 1; });

            _stateMachine.AddTransitionFor(armedWeaponController, idle, () => !armedWeaponController.Enabled);
            //_stateMachine.AddTransitionFor(armedWeaponController, weaponSwitching, length, () => _input.Supply, (s, d, t) =>
            //{
            //    acore_new.StatusNum = 2;
            //    if (t >= 0.3f)
            //    {
            //        acore_new.ReplaySwitching();
            //    }
            //    acore_new.SwitchingWeight = t;
            //}, 0);
            _stateMachine.AddTransition(transition_ats);

            _stateMachine.AddTransitionFor(weaponSwitching, idle, () => !armedWeaponController.Enabled);
            //_stateMachine.AddTransitionFor(weaponSwitching, armedWeaponController, length, () => armedWeaponController.Enabled, (s, d, t) =>
            //{
            //    acore_new.StatusNum = 2;
            //    acore_new.PauseSwitching();
            //    acore_new.SwitchingWeight = 1 - t;
            //}, 0);
            _stateMachine.AddTransition(transition_sta);
        }


        void SetDefaultWeapon(MountPoint weaponMountPoint, string weaponName, WeaponCore weaponCore)
        {
            if (!weaponCore.TryGetWeaponObj(weaponName, out var obj))
                throw new Exception();
            var weapon = obj.GetComponent<IWeapon>();
            //armedWeaponController.ActivateBehaviourBy(weapon);
            weaponMountPoint.LoadObj = obj;

        }
        int ctx = 0;
        public override void OnUpdate()
        {
            if (_stateMachine.CurrentState != null && _stateMachine.CurrentState.Name == "arm_armed_weapon ->  arm_switching")
            {
                if (ctx > 1)
                    Debug.Log("dpoint");
                ctx = 1;
            }
            if (ctx > 0 && _stateMachine.CurrentState.Name != "arm_armed_weapon ->  arm_switching")
                ctx = 2;
            _stateMachine.OnUpdate();
            Debug.Log("ArmCore.StateMachine: " + _stateMachine);
        }

        public override void OnEnter()
        {
            enabled = true;
        }

        public override void OnExit()
        {
            enabled = false;
        }
        void Update()
        {
            this.OnUpdate();
            acore_new.OnUpdate();
            //animationCore.OnUpdate();
        }

        public Playable GetPlayablePart(PlayableGraph graph)
        {
            //return animationCore.GetPlayablePart(graph);
            return default;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
