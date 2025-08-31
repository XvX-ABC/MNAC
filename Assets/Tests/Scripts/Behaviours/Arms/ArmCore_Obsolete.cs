using System;
using System.Collections.Generic;
using Tests.Behaviours.Animations;
using Tests.Behaviours.Arms.Animations;
using Tests.Behaviours.Arms.Weapons;
using Tests.Characters;
using Tests.Characters.Arms;
using Tests.Characters.Arms.Animations;
using Tests.Input;
using Tests.States;
using Tests.Weapons;
using UnityEngine;
using UnityEngine.Playables;
using ArmAnimationCore_Obsolete = Tests.Behaviours.Arms.Animations.ArmAnimationCore_Obsolete;

namespace Tests.Behaviours.Arms
{

    [RequireComponent(typeof(ArmDefinitions))]
    [RequireComponent(typeof(ArmAnimationDefinitions))]
    [Obsolete]
    public class ArmCore_Obsolete : State_MonoComponent, IArmBehaviour, IState
    {
        internal class IdleState : ArmPlayableState_Obsolete
        {
            ArmAnimationCore_Obsolete _core;
            internal ArmAnimationCore_Obsolete animationCore { set => _core = value; }
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

        public static implicit operator StateBase<object>(ArmCore_Obsolete core)
        {
            return core._stateMachine;
        }

        WeaponCore _weaponCore;
        IInput _input;
        [SerializeField]
        MountPoint[] _mountPoints;

        internal IArmDefinitions definitions;
        internal IArmAnimationDefinitions animationDefinitions;


        internal WeaponSwitching_Obsolete weaponSwitching;
        internal ArmedWeaponArmBehavioursController_Obsolete armedWeaponController;
        internal IdleState idle;
        AnimationTransition transition_its;
        AnimationTransition transition_ats;
        AnimationBlendingTransition transition_sta;
        BlendingTransition<object> transition_sti;

        //internal ArmAnimationCore animationCore;
        internal ArmAnimationCore_Obsolete acore_new;
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
            definitions = GetComponent<ArmDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ArmDefinitions));
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
            //transition_ats.animationCore = acore_new;
            //transition_sta.animationCore = acore_new;

            SetDefaultWeapon(weaponMountPoint, weaponDefinitions.Origins[0].Name, _weaponCore);

        }
        void InitializeSwitchingBehaviour(IArmWeaponDefinitions definitions, MountPoint mountPoint)
        {
            weaponSwitching = new WeaponSwitching_Obsolete(definitions, mountPoint, _weaponCore);
        }
        void InitializeArmedWeaponBehaviours(IArmWeaponDefinitions definitions)
        {
            var blist = new List<(string, IArmedWeaponArmBehaviour_Obsolete)>();
            var behaviours = this.GetComponents<IArmedWeaponArmBehaviour_Obsolete>();
            armedWeaponController = new ArmedWeaponArmBehavioursController_Obsolete(_weaponCore, definitions, behaviours);


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
