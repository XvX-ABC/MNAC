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

    internal class AnimationBlendingTransition : BlendingTransition<object>
    {
        internal ArmAnimationCore_New animationCore;
        int _statusNum;
        public AnimationBlendingTransition(
            int statusNum,
           IWithCallbackPlayableState<object> sourceState,
           IWithCallbackPlayableState<object> destinationState,
           Func<bool> triggerEvent,
           Action<IPlayableState<object>, IPlayableState<object>, float> durationEvent,
           float duration,
           float offset = 0,
           float fixedExitTime = FIXED_EXIT_TIME_INVALID_VALUE,
           InterruptionSource interruptionSource = INTERRUPTION_SOURCE_DEFAULT) : base(sourceState, destinationState, triggerEvent, durationEvent, duration, offset, fixedExitTime, interruptionSource)
        {
            this._statusNum = statusNum;
        }
        protected override void Begin(TimelineContext ctx)
        {
            base.Begin(ctx);
            if (animationCore == null)
                return;
            animationCore.StatusNum = (byte)_statusNum;
        }
    }
    internal class AnimationTransition : AnimationBlendingTransition
    {
        public AnimationTransition(
            int statusNum,
            IWithCallbackPlayableState<object> sourceState,
            IAnimationPlayableState destinationState,
            Func<bool> triggerEvent,
            Action<IPlayableState<object>, IAnimationPlayableState, float> durationEvent,
            float duration,
            float offset = 0,
            float fixedExitTime = FIXED_EXIT_TIME_INVALID_VALUE,
            InterruptionSource interruptionSource = INTERRUPTION_SOURCE_DEFAULT) : base(statusNum, sourceState, destinationState, triggerEvent, null, duration, offset, fixedExitTime, interruptionSource)
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
            base.Begin(ctx);
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
    public class ArmCore : StateMonoComponentBase, IArmBehaviour, IState
    {
        internal class IdleState : ArmPlayableState
        {
            ArmAnimationCore_New _core;
            internal ArmAnimationCore_New animationCore { set => _core = value; }
            public IdleState() : base("idle")
            {
            }

            public override void OnEnter()
            {
                base.OnEnter();
                //_core.StatusNum = 3;
            }

            public override void OnExit()
            {
                base.OnExit();
            }

            public override void OnUpdate()
            {
                base.OnUpdate();
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
        AnimationTransition transition_its;
        AnimationTransition transition_ats;
        AnimationBlendingTransition transition_sta;
        BlendingTransition<object> transition_sti;

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

            weaponSwitching.animationCore = acore_new;
            armedWeaponController.AnimationCore = acore_new;
            idle.animationCore = acore_new;
            transition_ats.animationCore = acore_new;
            transition_sta.animationCore = acore_new;

            SetDefaultWeapon(weaponMountPoint, weaponDefinitions.Origins[0].Name, _weaponCore);

        }
        void InitializeSwitchingBehaviour(IArmWeaponDefinitions definitions, MountPoint mountPoint)
        {
            weaponSwitching = new ArmWeaponSwitching(definitions, mountPoint, _weaponCore);
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

            //var length = definitions.Weapon.SwitchingToBehavioursDurationTime;
            var length = 10;





            #region from idle to other states
            transition_its = new(0, idle, weaponSwitching, () => _input.Supply, null, 0.07f, 0, 0, InterruptionSource.Next);
            //_stateMachine.AddTransitionFor(idle, weaponSwitching, 0.05f, () => _input.Supply, (_, _, t) =>
            //{
            //});
            _stateMachine.AddTransition(transition_its);
            _stateMachine.AddTransitionFor(idle, armedWeaponController, length / 4, () => armedWeaponController.EntryFunc(), (_, _, t) =>
            {
                //acore_new.StatusNum = 1;
            });
            #endregion

            #region from armed weapon to other states

            transition_ats = new AnimationTransition(2, armedWeaponController, weaponSwitching, () => _input.Supply, (s, d, t) =>
            {
            }, 1, 0, -1, InterruptionSource.None);
            _stateMachine.AddTransitionFor(armedWeaponController, idle, length / 4, () => armedWeaponController.ExitFunc(), (s, d, t) =>
            {
                acore_new.StatusNum = 1;
            });
            _stateMachine.AddTransition(transition_ats);
            #endregion


            #region from switching to other states
            transition_sta = new(2, weaponSwitching, armedWeaponController, () => armedWeaponController.EntryFunc(), (s, d, t) =>
            {
            }, 1, 0, 0.5f, InterruptionSource.Next);
            transition_sti = new(weaponSwitching, idle, () => !armedWeaponController.EntryFunc(), (_, _, t) => acore_new.StatusNum = 0, 0.07f, 0, 1, InterruptionSource.None);
            //_stateMachine.AddTransitionFor(weaponSwitching, idle, 1, () => armedWeaponController.ExitFunc(), null);
            _stateMachine.AddTransition(transition_sti);
            _stateMachine.AddTransition(transition_sta);
            #endregion
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
            //if (_stateMachine.CurrentState != null && _stateMachine.CurrentState.Name == "arm_armed_weapon ->  arm_switching")
            //{
            //    Debug.Log("dpoint");
            //}
            //if (_stateMachine.CurrentState != null && _stateMachine.CurrentState.Name == "arm_switching")
            //{
            //    Debug.Log("debug point");
            //}
            _stateMachine.OnUpdate();
            //Debug.Log("Arm core state machine: " + _stateMachine);
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
