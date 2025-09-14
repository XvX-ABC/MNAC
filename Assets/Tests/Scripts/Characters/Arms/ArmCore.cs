using System;
using Tests.Behaviours;
using Tests.Behaviours.Animations;
using Tests.Behaviours.Arms;
using Tests.Behaviours.Arms.Animations;
using Tests.Behaviours.Arms.Weapons.Animations;
using Tests.Characters.Arms.Weapons;
using Tests.Input;
using Tests.States;
using Tests.Weapons;
using UnityEngine;
using UnityEngine.Playables;
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
            return core.stateMachine;
        }

        WeaponCore _weaponCore;
        IInput _input;
        [SerializeField]
        MountPoint[] _mountPoints;

        IArmDefinitions _definitions;
        IArmAnimationDefinitions animationDefinitions;


        internal WeaponSwitchingState weaponSwitching;
        internal ArmedWeaponArmBehaviourControllerState armedWeaponController;
        internal IdleState idle;
        AnimationTransition transition_its;
        AnimationTransition transition_ats;
        AnimationBlendingTransition transition_sta;
        BlendingTransition<object> transition_sti;

        //internal ArmAnimationCore animationCore;
        internal ArmAnimationCore animatorCore;
        Blackboard _blackboard;
        ComponentNode _node;
        internal PlayableStateMachine stateMachine;
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

        public IOutputSetting OutputSetting { get => animatorCore.OutputSetting; set => animatorCore.OutputSetting = value; }
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

            if (!blackboard.TryReadValue<PlayableGraph>(CharacterBlackboardFields.Character_Animation_Graph, out var graph)) ;

            var weaponDefinitions = _definitions.Weapon;
            var weaponMountPoint = FindMountPoint(weaponDefinitions.MountPointName) ?? throw new CantFindMountPointByNameException(weaponDefinitions.MountPointName);

            InitializeSwitchingBehaviour(weaponDefinitions, weaponMountPoint);

            InitializeArmedWeaponBehaviours(weaponDefinitions);



            InitializeChildNodes();

            this.animatorCore = new(graph, _definitions.Weapon, animationDefinitions.Weapon, new ArmedWeaponArmAnimator<IArmedWeaponArmBehaviour>(graph, this.armedWeaponController));
            InitializeStateMachine();

            weaponSwitching.animationCore = animatorCore;
            armedWeaponController.animationCore = animatorCore;
            idle.animationCore = animatorCore;
            transition_ats.animationCore = animatorCore;
            transition_sta.animationCore = animatorCore;

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
            stateMachine = new(this.name);
            stateMachine.AddState(idle);
            stateMachine.AddState(armedWeaponController);
            stateMachine.AddState(weaponSwitching);

            var length = 10;




            #region from idle to other states
            transition_its = new(0, idle, weaponSwitching, () => _input.Supply, null, 0.07f, 0, 0, InterruptionSource.Next);
            stateMachine.AddTransitionFor(transition_its);
            stateMachine.AddTransitionFor(idle, armedWeaponController, 0.3f, () => armedWeaponController.EntryFunc(), null);
            #endregion

            #region from armed weapon to other states

            transition_ats = new AnimationTransition(2, armedWeaponController, weaponSwitching, () => _input.Supply, null, 1, 0, -1, InterruptionSource.None);
            stateMachine.AddTransitionFor(armedWeaponController, idle, 0.3f, () => armedWeaponController.ExitFunc(), null);
            stateMachine.AddTransitionFor(transition_ats);
            #endregion

            #region from switching to other states
            transition_sta = new(2, weaponSwitching, armedWeaponController, () => armedWeaponController.EntryFunc(), (s, d, t) =>
            {
            }, 1, 0, 0.5f, InterruptionSource.Next);
            transition_sti = new(weaponSwitching, idle, () => !armedWeaponController.EntryFunc(), (_, _, t) => animatorCore.StatusNum = 0, 0.07f, 0, 1, InterruptionSource.None);
            stateMachine.AddTransitionFor(transition_sti);
            stateMachine.AddTransitionFor(transition_sta);
            #endregion
        }


        void SetDefaultWeapon(MountPoint weaponMountPoint, string weaponName, WeaponCore weaponCore)
        {
            if (!weaponCore.TryGetWeaponObj(weaponName, out var obj))
                throw new Exception();
            weaponMountPoint.LoadObj = obj;

        }
        public override void OnUpdate()
        {
            stateMachine.OnUpdate();
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
            animatorCore.OnUpdate();
        }

        public void Dispose()
        {
        }
    }
}
