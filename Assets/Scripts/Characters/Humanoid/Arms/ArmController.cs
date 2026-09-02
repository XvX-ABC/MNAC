using System;
using MNAC.Animations;
using MNAC.Behaviours.Arms.Animations;
using MNAC.Behaviours.Arms.Weapons;
using MNAC.Behaviours.Arms.Weapons.Animations;
using MNAC.Characters.Humanoid.Arms.Weapons;
using MNAC.Characters.Humanoid.Input;
using MNAC.Characters.MountPoints;
using MNAC.States;
using MNAC.Utilities.Blackboards;
using MNAC.Utilities.Timeline;
using MNAC.Weapons;
using MNAC.Weapons;
using UnityEngine;
using UnityEngine.Playables;
using IArmedArmBehaviour = MNAC.Characters.Humanoid.Arms.Weapons.IArmedArmBehaviour;
using IArmedArmDefinitions = MNAC.Characters.Humanoid.Arms.Weapons.IArmedArmDefinitions;
using WeaponBackpack = MNAC.Characters.Weapons.WeaponBackpack;

using MNAC.Utilities;
namespace MNAC.Characters.Humanoid.Arms
{
    internal class ArmController : HumanoidComponent, IArmBehaviour
    {
        #region internal classes
        internal class IdleState : WithCallbackPlayableState
        {
            ArmAnimationCore _core;
            internal ArmAnimationCore animationCore { set => _core = value; }
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
        #endregion
        public static implicit operator StateBase<object>(ArmController core)
        {
            return core.stateMachine;
        }


        WeaponBackpack _weaponBackpack;
        IArmInput _armInput;
        [SerializeField]
        MountPoint[] _mountPoints;
        [SerializeField]
        HumanBodyPart _part;


        IArmDefinitions _definitions;
        IArmAnimationDefinitions animationDefinitions;


        internal WeaponSwitchingState weaponSwitching;
        Func<WeaponDescription[], string> _weaponSelectionFunc;


        ArmedArmBehaviourController<IArmedArmBehaviour> _armedArmBehaviourController;
        internal ArmedArmBehaviourControllerState armedWeaponControllerState;


        internal IdleState idle;
        internal AnimationTransition transition_its;
        internal AnimationTransition transition_ats;
        internal IPlayableTransition<object> transition_ati;
        internal AnimationBlendingTransition transition_sta;
        internal BlendingTransition<object> transition_sti;

        internal PlayableStateMachine stateMachine;


        internal ArmAnimationCore animatorCore;


        //public IOutputSetting OutputSetting { get => animatorCore.OutputSetting; set => animatorCore.OutputSetting = value; }
        public Action<Playable> UpdateAction { get => throw new Exception(); set => throw new Exception(); }


        internal IArmDefinitions definitions { get => _definitions; set => _definitions = value; }
        internal Func<WeaponDescription[], string> weaponSelectionFunc
        {
            get => _weaponSelectionFunc;
            set
            {
                if (weaponSwitching != null)
                {
                    weaponSwitching.switching.selectionFunc = value;
                }
                _weaponSelectionFunc = value;
            }
        }
        internal ITimeline weaponSwitchingTimeline
        {
            get => weaponSwitching.switching.timeline;
        }
        internal IWeapon currentWeapon { get => weaponSwitching?.switching?.CurrentWeapon; }
        internal WeaponBackpack weaponBackpack { get => _weaponBackpack; }
        internal IArmedArmBehaviour currentActivatedBehaviour { get => armedWeaponControllerState.currentActivatedBehaviour; }
        internal HumanBodyPart bodyPart
        {
            get => _part;
            set
            {
                _part = value;
                if (_part != HumanBodyPart.LeftArm && _part != HumanBodyPart.RightArm)
                    throw new Exception("The body part must be either the left arm or the right arm.");
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _definitions = GetComponent<IArmDefinitions>() ?? throw new ComponentCantFindException(gameObject, typeof(IArmDefinitions));
            animationDefinitions = _definitions.Animation;

            bodyPart = _part;
        }
        void OnEnable()
        {
            if (stateMachine != null)
            {
                animatorCore.StatusNum = 3;
                stateMachine.Enabled = true;
            }
        }
        void OnDisable()
        {
            if (stateMachine != null)
            {
                stateMachine.Enabled = false;
                animatorCore.StatusNum = 3;
            }
        }
        void Update()
        {
            stateMachine.OnUpdate();
            _armedArmBehaviourController.Update();
            animatorCore.OnUpdate();
        }
        void FixedUpdate()
        {
            _armedArmBehaviourController.FixedUpdate();
        }
        void LateUpdate()
        {
            _armedArmBehaviourController.LateUpdate();
        }


        internal MountPoint FindMountPoint(string name)
        {
            foreach (var m in _mountPoints)
            {
                if (m.Name == name)
                    return m;
            }
            return null;
        }
        internal MountPoint FindMountPoint(MountPointLocation fieldEnum)
        {
            foreach (var m in _mountPoints)
                if (m.place == fieldEnum)
                    return m;
            return null;
        }
        void InitializeChildNodes()
        {
            node.AddChild(weaponSwitching.Node);
            node.AddChild(armedWeaponControllerState.Node);
        }
        void RegisterMountPoints(Blackboard blackboard)
        {
            foreach (var m in _mountPoints)
            {
                blackboard.TryRegisterMountPoint(m.place, m);
            }
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryReadValueOrThrowException<IHumanoidInput>(CharacterBlackboardFields.Character_Input_Main, out var input);
            _armInput = _part switch
            {
                HumanBodyPart.LeftArm => input.LArm,
                HumanBodyPart.RightArm => input.RArm,
            };

            blackboard.TryReadValueOrThrowException(CharacterBlackboardFields.Character_Weapon_Backpack, out _weaponBackpack);
            if (!blackboard.TryReadValue<PlayableGraph>(CharacterBlackboardFields.Character_Animation_Graph, out var graph))
                throw new Exception();


            if (_part == HumanBodyPart.LeftArm)
                owner.leftArm = this;
            else if (_part == HumanBodyPart.RightArm)
                owner.rightArm = this;
            this.Enabled = true;



            var weaponDefinitions = _definitions.Weapon;
            var launcherMountPoint = FindMountPoint(weaponDefinitions.MountPoints.Launcher) ?? throw new NullReferenceException("launcherMountPoint");
            var swordMountPoint = FindMountPoint(weaponDefinitions.MountPoints.Sword) ?? throw new NullReferenceException("swordMountPoint");
            RegisterMountPoints(blackboard);

            InitializeSwitchingBehaviour(weaponDefinitions, launcherMountPoint, swordMountPoint);

            InitializeArmedWeaponBehaviours(weaponDefinitions);




            animatorCore = new(graph, _definitions.Weapon, animationDefinitions.Weapon, new ArmedArmAnimator<IArmedArmBehaviour>(graph, armedWeaponControllerState));
            InitializeStateMachine();

            weaponSwitching.animationCore = animatorCore;
            armedWeaponControllerState.animationCore = animatorCore;
            idle.animationCore = animatorCore;
            transition_ats.animationCore = animatorCore;
            transition_sta.animationCore = animatorCore;



            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Obj_Arm_Local, this.gameObject);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Arm_Core_Local, this);
            var field = _part switch
            {
                HumanBodyPart.None => Guid.Empty,
                HumanBodyPart.LeftArm => CharacterBlackboardFields.Character_Arm_Left_Controller,
                HumanBodyPart.RightArm => CharacterBlackboardFields.Character_Arm_Right_Controller,
                _ => throw new NotImplementedException()
            };
            blackboard.TryRegisterField(field, this);





            InitializeChildNodes();

            SetDefaultWeapon(launcherMountPoint, weaponDefinitions.Origins[0].Name, _weaponBackpack);

        }
        void InitializeSwitchingBehaviour(IArmedArmDefinitions definitions, MountPoint launcherMountPoint, MountPoint swordMountPoint)
        {
            weaponSwitching = new(this, definitions, launcherMountPoint, swordMountPoint, _weaponBackpack);
            weaponSwitching.switching.selectionFunc = _weaponSelectionFunc;
        }
        void InitializeArmedWeaponBehaviours(IArmedArmDefinitions definitions)
        {
            var behaviours = _definitions.Weapon.ArmedWeaponBehaviours;
            _armedArmBehaviourController = new(_definitions.Weapon, behaviours);
            armedWeaponControllerState = new(_armedArmBehaviourController, _part);
            weaponSwitching.SwitchedEvent += (ow, nw) =>
            {
                if (ow != null)
                {
                    armedWeaponControllerState.UnactivateBehaviourBy(ow);
                }
                if (nw != null)
                {
                    var field = _part switch
                    {
                        HumanBodyPart.LeftArm => CharacterBlackboardFields.Character_Weapon_LeftArm_Armed,
                        HumanBodyPart.RightArm => CharacterBlackboardFields.Character_Weapon_RightArm_Armed,
                    };
                    if (blackboard.Contains(field))
                        blackboard.TryWriteValue(field, nw);
                    else
                        blackboard.TryRegisterField(field, nw);
                    armedWeaponControllerState.ActivateBehaviourBy(nw);
                }

            };
        }
        void InitializeStateMachine()
        {
            idle = new IdleState();
            stateMachine = new(name);
            stateMachine.AddState(idle);
            stateMachine.AddState(armedWeaponControllerState);
            stateMachine.AddState(weaponSwitching);

            #region from idle to other states
            transition_its = new(0, idle, weaponSwitching, () => _armInput.WeaponSwitch, null, 0.07f, 0, AnimationTransition.FIXED_EXIT_TIME_INVALID_VALUE, InterruptionSource.Next);
            stateMachine.AddTransitionFor(transition_its);
            stateMachine.AddTransitionFor(idle, armedWeaponControllerState, 1, () => armedWeaponControllerState.ActivationTrigger(), null);
            #endregion

            #region from armed weapon to other states

            transition_ats = new AnimationTransition(2, armedWeaponControllerState, weaponSwitching, () => _armInput.WeaponSwitch, null, 1, 0, AnimationTransition.FIXED_EXIT_TIME_INVALID_VALUE, InterruptionSource.None);
            transition_ati = stateMachine.AddTransitionFor(armedWeaponControllerState, idle, 0.3f, () => armedWeaponControllerState.UnactivationTrigger(), null);
            stateMachine.AddTransitionFor(transition_ats);
            #endregion

            #region from switching to other states
            transition_sta = new(2, weaponSwitching, armedWeaponControllerState, () => armedWeaponControllerState.ActivationTrigger(), (s, d, t) =>
            {
            }, 1, 0, 0.5f, InterruptionSource.Next);
            transition_sti = new(weaponSwitching, idle, () => !armedWeaponControllerState.ActivationTrigger(), (_, _, t) => animatorCore.StatusNum = 0, 0.07f, 0, 1, InterruptionSource.None);
            stateMachine.AddTransitionFor(transition_sti);
            stateMachine.AddTransitionFor(transition_sta);
            #endregion
        }


        void SetDefaultWeapon(MountPoint weaponMountPoint, string weaponName, WeaponBackpack backpack)
        {
            weaponSwitching.SetDefaultWeapon();
        }


    }
}
