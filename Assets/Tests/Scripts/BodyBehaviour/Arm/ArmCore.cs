using Assets.Tests.Scripts.BodyBehaviour.Arm.Animations;
using Assets.Tests.Scripts.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Behaviours.Arm.Weapons;
using Tests.BodyBehaviour.Arm.Animations;
using Tests.Characters;
using Tests.Input;
using Tests.Locomotion.Animation;
using Tests.States;
using Tests.Weapons;
using UnityEngine;
using UnityEngine.Playables;
using static UnityEngine.Rendering.DebugUI;

namespace Tests.Behaviours.Arm
{

    [RequireComponent(typeof(ArmBehaviourDefinitions))]
    [RequireComponent(typeof(ArmAnimationDefinitions))]
    public class ArmCore : StateMonoComponentBase, IArmBehaviour, IState, IDynamicPlayablePart
    {
        internal class IdleState : PlayableStateBase
        {
            ArmAnimationCore_New _core;
            internal ArmAnimationCore_New core { set => _core = value; }
            public IdleState() : base("idle")
            {
            }

            public override void OnEnter()
            {
                _core.StatusNum = 4;
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
            idle.core = acore_new;
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

            _stateMachine.AddTransitionFor(idle, weaponSwitching, () => _input.Supply);
            _stateMachine.AddTransitionFor(idle, armedWeaponController, 0, () => armedWeaponController.Enabled, (_, _, _) => { acore_new.StatusNum = 1; });

            _stateMachine.AddTransitionFor(armedWeaponController, idle, () => !armedWeaponController.Enabled);
            _stateMachine.AddTransitionFor(armedWeaponController, weaponSwitching, length, () => _input.Supply, (s, d, t) =>
            {
                acore_new.StatusNum = 2;
                if (t >= 0.3f)
                {
                    acore_new.StartPlaySwitching();
                }
                acore_new.SwitchingWeight = t;
            }, 0);

            _stateMachine.AddTransitionFor(weaponSwitching, idle, () => !armedWeaponController.Enabled);
            _stateMachine.AddTransitionFor(weaponSwitching, armedWeaponController, length, () => armedWeaponController.Enabled, (s, d, t) =>
            {
                acore_new.StatusNum = 2;
                acore_new.StopPlaySwitching();
                acore_new.SwitchingWeight = 1 - t;
            }, 0);
        }


        void SetDefaultWeapon(MountPoint weaponMountPoint, string weaponName, WeaponCore weaponCore)
        {
            if (!weaponCore.TryGetWeaponObj(weaponName, out var obj))
                throw new Exception();
            var weapon = obj.GetComponent<IWeapon>();
            //armedWeaponController.ActivateBehaviourBy(weapon);
            weaponMountPoint.LoadObj = obj;

        }

        public override void OnUpdate()
        {
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
