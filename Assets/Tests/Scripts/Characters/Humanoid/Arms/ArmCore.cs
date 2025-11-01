using System;
using Tests.Animations;
using Tests.Behaviours.Arms;
using Tests.Behaviours.Arms.Animations;
using Tests.Behaviours.Arms.Weapons;
using Tests.Behaviours.Arms.Weapons.Animations;
using Tests.Characters.Humanoid.Arms.Weapons;
using Tests.Characters.Humanoid.Interaction.Input;
using Tests.Characters.MountPoints;
using Tests.States;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using Tests.Weapons;
using UnityEngine;
using UnityEngine.Playables;
using IArmedWeaponArmBehaviour = Tests.Characters.Humanoid.Arms.Weapons.IArmedWeaponArmBehaviour;
using IArmedWeaponArmDefinitions = Tests.Characters.Humanoid.Arms.Weapons.IArmedWeaponArmDefinitions;
using MountPoint = Tests.Characters.MountPoints.MountPoint;

namespace Tests.Characters.Humanoid.Arms
{
    [RequireComponent(typeof(ArmDefinitions_MonoComponent))]
    [RequireComponent(typeof(ArmAnimationDefinitions_MonoComponent))]
    public class ArmCore : State_MonoComponent, IArmBehaviour
    {
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

        public static implicit operator StateBase<object>(ArmCore core)
        {
            return core.stateMachine;
        }

        WeaponCore _weaponCore;
        IArmInput _armInput;
        [SerializeField]
        MountPoint[] _mountPoints;
        [SerializeField]
        ArmedWeaponArmBehaviourBase_SO[] _behaviours;

        IArmDefinitions _definitions;
        IArmAnimationDefinitions animationDefinitions;


        internal WeaponSwitchingState weaponSwitching;
        ArmedWeaponArmBehaviourController<IArmedWeaponArmBehaviour> _armedWeaponController;
        internal ArmedWeaponArmBehaviourControllerState armedWeaponControllerState;
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
                    value.TryReadValueOrThrowException<IHumanInput>(CharacterBlackboardFields.Character_Input_Main, out var input);
                    _armInput = _definitions.Part == HumanPart.LeftArm ? input.LArm : input.RArm;
                    value.TryReadValue(CharacterBlackboardFields.Character_Weapon_Core, out _weaponCore);
                }
                _blackboard = value;
            }
        }
        public IComponentNode Node
        {
            get => _node;
        }

        public IOutputSetting OutputSetting { get => animatorCore.OutputSetting; set => animatorCore.OutputSetting = value; }
        public Action<Playable> UpdateAction { get => throw new Exception(); set => throw new Exception(); }
        protected override void Awake()
        {
            base.Awake();
            _definitions = GetComponent<ArmDefinitions_MonoComponent>() ?? throw new ComponentCantFindException(gameObject, typeof(ArmDefinitions_MonoComponent));
            //animationDefinitions = GetComponent<ArmAnimationDefinitions_MonoComponent>() ?? throw new ComponentCantFindException(gameObject, typeof(IArmAnimationDefinitions));
            animationDefinitions = _definitions.Animation;

            _node = new(this);

            if (_definitions.Part != HumanPart.LeftArm && _definitions.Part != HumanPart.RightArm)
                throw new Exception("The part of definitions must is left arm or right arm.");
        }

        private void Start()
        {
            var mp = FindMountPoint(_definitions.Weapon.MountPointName);
            var n = _definitions.Weapon.Origins[0].Name;

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
        void InitializeChildNodes()
        {
            _node.AddChild(weaponSwitching.Node);
            _node.AddChild(armedWeaponControllerState.Node);
        }
        public void Initialize(Blackboard blackboard)
        {
            Blackboard = blackboard;

            if (!blackboard.TryReadValue<PlayableGraph>(CharacterBlackboardFields.Character_Animation_Graph, out var graph))
                throw new Exception();
            //blackboard.TryRegisterField(CharacterBlackboardFields.Character_Obj_Arm_Local, this.gameObject);
            //blackboard.TryRegisterField(CharacterBlackboardFields.Character_Arm_Core_Local, this);
            blackboard.TryRegisterFieldOrWriteValue(CharacterBlackboardFields.Character_Obj_Arm_Local, this.gameObject);
            blackboard.TryRegisterFieldOrWriteValue(CharacterBlackboardFields.Character_Arm_Core_Local, this);

            var weaponDefinitions = _definitions.Weapon;
            var weaponMountPoint = FindMountPoint(weaponDefinitions.MountPointName) ?? throw new CantFindMountPointByNameException(weaponDefinitions.MountPointName);
            weaponMountPoint.field = _definitions.Part switch
            {
                HumanPart.LeftArm => MountPointFields.Enum.Left_Arm_Hand_Weapon,
                HumanPart.RightArm => MountPointFields.Enum.Right_Arm_Hand_Weapon,
                _ => throw new Exception("The part of definitions must is left arm or right arm.")
            };

            blackboard.TryRegisterMountPoint(weaponMountPoint);

            InitializeSwitchingBehaviour(weaponDefinitions, weaponMountPoint);

            InitializeArmedWeaponBehaviours(weaponDefinitions);



            InitializeChildNodes();
            animatorCore = new(graph, _definitions.Weapon, animationDefinitions.Weapon, new ArmedWeaponArmAnimator<IArmedWeaponArmBehaviour>(graph, armedWeaponControllerState));
            InitializeStateMachine();

            weaponSwitching.animationCore = animatorCore;
            armedWeaponControllerState.animationCore = animatorCore;
            idle.animationCore = animatorCore;
            transition_ats.animationCore = animatorCore;
            transition_sta.animationCore = animatorCore;

            SetDefaultWeapon(weaponMountPoint, weaponDefinitions.Origins[0].Name, _weaponCore);

        }
        void InitializeSwitchingBehaviour(IArmedWeaponArmDefinitions definitions, MountPoint mountPoint)
        {
            weaponSwitching = new(definitions, mountPoint, _weaponCore);
        }
        void InitializeArmedWeaponBehaviours(IArmedWeaponArmDefinitions definitions)
        {
            //var behaviours = GetComponents<IArmedWeaponArmBehaviour>();
            var behaviours = _behaviours;
            //armedWeaponController = new(_weaponCore, definitions, behaviours);
            //armedWeaponControllerState = new(_weaponCore, definitions, _definitions.Part, behaviours);
            _armedWeaponController = new(_weaponCore, _definitions.Weapon, behaviours);
            armedWeaponControllerState = new(_armedWeaponController, _definitions.Part);


            weaponSwitching.SwitchingEvent += (ow, nw) =>
            {
                if (ow != null)
                {
                    armedWeaponControllerState.UnactivateBehaviourBy(ow);
                }
                if (nw != null)
                {
                    //var field = Guid.Empty;
                    //switch (_definitions.Part)
                    //{
                    //    case HumanPart.LeftArm:
                    //        field = CharacterBlackboardFields.Character_Weapon_LeftArm_Armed;
                    //        break;
                    //    case HumanPart.RightArm:
                    //        field = CharacterBlackboardFields.Character_Weapon_RightArm_Armed;
                    //        break;
                    //}
                    var field = _definitions.Part switch
                    {
                        HumanPart.LeftArm => CharacterBlackboardFields.Character_Weapon_LeftArm_Armed,
                        HumanPart.RightArm => CharacterBlackboardFields.Character_Weapon_RightArm_Armed,
                    };
                    if (_blackboard.Contains(field))
                        _blackboard.TryWriteValue(field, nw);
                    else
                        _blackboard.TryRegisterField(field, nw);
                    armedWeaponControllerState.ActivateBehaviourBy(nw);
                }

                return nw;
            };
        }
        void InitializeStateMachine()
        {
            idle = new IdleState();
            stateMachine = new(name);
            stateMachine.AddState(idle);
            stateMachine.AddState(armedWeaponControllerState);
            stateMachine.AddState(weaponSwitching);

            var length = 10;




            #region from idle to other states
            transition_its = new(0, idle, weaponSwitching, () => _armInput.WeaponSwitch, null, 0.07f, 0, AnimationTransition.FIXED_EXIT_TIME_INVALID_VALUE, InterruptionSource.Next);
            //transition_its = new(0, idle, weaponSwitching, () => _input.Supply, null, 0.07f, 0, AnimationTransition.FIXED_EXIT_TIME_INVALID_VALUE, InterruptionSource.Next);
            stateMachine.AddTransitionFor(transition_its);
            stateMachine.AddTransitionFor(idle, armedWeaponControllerState, 0.3f, () => armedWeaponControllerState.EntryFunc(), null);
            #endregion

            #region from armed weapon to other states

            transition_ats = new AnimationTransition(2, armedWeaponControllerState, weaponSwitching, () => _armInput.WeaponSwitch, null, 1, 0, AnimationTransition.FIXED_EXIT_TIME_INVALID_VALUE, InterruptionSource.None);
            //transition_ats = new AnimationTransition(2, armedWeaponController, weaponSwitching, () => _input.Supply, null, 1, 0, AnimationTransition.FIXED_EXIT_TIME_INVALID_VALUE, InterruptionSource.None);
            stateMachine.AddTransitionFor(armedWeaponControllerState, idle, 0.3f, () => armedWeaponControllerState.ExitFunc(), null);
            stateMachine.AddTransitionFor(transition_ats);
            #endregion

            #region from switching to other states
            transition_sta = new(2, weaponSwitching, armedWeaponControllerState, () => armedWeaponControllerState.EntryFunc(), (s, d, t) =>
            {
            }, 1, 0, 0.5f, InterruptionSource.Next);
            transition_sti = new(weaponSwitching, idle, () => !armedWeaponControllerState.EntryFunc(), (_, _, t) => animatorCore.StatusNum = 0, 0.07f, 0, 1, InterruptionSource.None);
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
            OnUpdate();
            _armedWeaponController.Update();
            animatorCore.OnUpdate();
        }

        void FixedUpdate()
        {
            _armedWeaponController.FixedUpdate();
        }
        public void Dispose()
        {
        }
    }
}
