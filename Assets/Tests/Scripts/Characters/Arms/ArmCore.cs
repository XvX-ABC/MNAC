using System;
using System.Collections.Generic;
using Tests.Behaviours.Animations;
using Tests.Behaviours.Arms;
using Tests.Behaviours.Arms.Animations;
using Tests.Behaviours.Arms.Weapons;
using Tests.Behaviours.Arms.Weapons.Animations;
using Tests.Characters.Arms.Animations;
using Tests.Characters.Arms.Weapons;
using Tests.Input;
using Tests.States;
using Tests.Weapons;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Playables;
using ArmAnimationCore = Tests.Characters.Arms.Animations.ArmAnimationCore;
using IArmWeaponDefinitions = Tests.Characters.Arms.Weapons.IArmWeaponDefinitions;

namespace Tests.Characters.Arms
{
    [RequireComponent(typeof(ArmDefinitions))]
    [RequireComponent(typeof(ArmAnimationDefinitions))]
    public class ArmCore : State_MonoComponent, IArmBehaviour
    {
        internal class IdleState : WithCallbackPlayableState
        {
            Behaviours.Arms.Animations.ArmAnimationCore _core;
            internal Behaviours.Arms.Animations.ArmAnimationCore animationCore { set => _core = value; }
            public IdleState() : base("idle")
            {
            }

            public override void OnEnter()
            {
                base.OnEnter();
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

        IArmDefinitions _definitions;
        IArmAnimationDefinitions animationDefinitions;


        //internal ArmWeaponSwitching weaponSwitching;
        //internal ArmedWeaponArmBehavioursController armedWeaponController;
        internal WeaponSwitchingState weaponSwitching;
        internal ArmedWeaponArmBehaviourControllerState armedWeaponController;
        internal IdleState idle;
        AnimationTransition transition_its;
        AnimationTransition transition_ats;
        AnimationBlendingTransition transition_sta;
        BlendingTransition<object> transition_sti;

        //internal ArmAnimationCore animationCore;
        internal Behaviours.Arms.Animations.ArmAnimationCore acore_new;
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
            _definitions = GetComponent<ArmDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ArmDefinitions));
            animationDefinitions = GetComponent<ArmAnimationDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IArmAnimationDefinitions));
            _node = new(this);
        }

        private void Start()
        {
            var mp = FindMountPoint(_definitions.Weapon.MountPointName);
            var n = _definitions.Weapon.Origins[0].Name;

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
            var weaponDefinitions = _definitions.Weapon;
            var weaponMountPoint = FindMountPoint(weaponDefinitions.MountPointName) ?? throw new CantFindMountPointByNameException(weaponDefinitions.MountPointName);


            InitializeSwitchingBehaviour(weaponDefinitions, weaponMountPoint);

            InitializeArmedWeaponBehaviours(weaponDefinitions);



            InitializeChildNodes();

            //this.animationCore = new(this);
            //this.acore_new = new(this, new ArmedWeaponArmAnimator<IArmedWeaponArmBehaviour>(this.armedWeaponController));
            this.acore_new = new(_definitions.Weapon, animationDefinitions.Weapon, new ArmedWeaponArmAnimator<IArmedWeaponArmBehaviour>(this.armedWeaponController));
            InitializeStateMachine();

            weaponSwitching.animationCore = acore_new;
            armedWeaponController.animationCore = acore_new;
            idle.animationCore = acore_new;
            transition_ats.animationCore = acore_new;
            transition_sta.animationCore = acore_new;

            SetDefaultWeapon(weaponMountPoint, weaponDefinitions.Origins[0].Name, _weaponCore);

        }
        void InitializeSwitchingBehaviour(IArmWeaponDefinitions definitions, MountPoint mountPoint)
        {
            weaponSwitching = new(definitions, mountPoint, _weaponCore);
        }
        void InitializeArmedWeaponBehaviours(IArmWeaponDefinitions definitions)
        {
            var behaviours = this.GetComponents<IArmedWeaponArmBehaviour>();
            armedWeaponController = new(_weaponCore, definitions, behaviours);


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
            _stateMachine.OnUpdate();
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

        public void Dispose()
        {
        }
    }
}
