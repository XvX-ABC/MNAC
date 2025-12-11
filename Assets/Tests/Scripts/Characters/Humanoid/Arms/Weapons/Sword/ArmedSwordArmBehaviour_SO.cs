using System;
using Tests.Animations;
using Tests.Behaviours;
using Tests.Behaviours.Arms.Weapons.Sword.Animations;
using Tests.Characters.Humanoid.Interaction.Input;
using Tests.Characters.Interaction;
using Tests.Characters.Interaction.Input;
using Tests.Characters.MountPoints;
using Tests.Interaction;
using Tests.States;
using Tests.Utilities.Blackboards;
using Tests.Utilities.MountPoints;
using Tests.Weapons_New;
using Tests.Weapons_New.Sword;
using UnityEngine;
using UnityEngine.Playables;
using LocomotionCore = Tests.Characters.Humanoid.Locomotion.LocomotionCore;
using Transition = Tests.Behaviours.Arms.Weapons.Sword.Animations.IArmedSwordArmAnimationDefinitions.Transition;
namespace Tests.Characters.Humanoid.Arms.Weapons.Sword
{


    [CreateAssetMenu(fileName = "ArmedSwordArmBehaviour", menuName = "Tests/Behaviours/Characters/Humanoid/Arms/Weapons/Sword/ArmedSwordArmBehaviour")]
    public class ArmedSwordArmBehaviour_SO : ArmedWeaponArmBehaviourBase_SO
    {
        //IArmedSwordArmBehaviourDefinitions _definitions;
        //IArmedSwordArmAnimationDefinitions _animationDefinitions;
        [SerializeField]
        ArmedSwordArmBehaviourDefinitions_SO _definitions;
        [SerializeField]
        ArmedSwordArmAnimationDefinitions_SO _animationDefinitions;

        Behaviours.Arms.Weapons.Sword.ArmedSwordArmBehaviour _behaviour;
        ArmedSwordArmAnimator _animator;

        ITargetLocker _targetLocker;
        SphericalObjsTrigger _targetsTrigger;
        LoadBase _load;

        BoostingHelper _boostingHelper;
        SlashHelper _slashHelper;

        WithCallbackPlayableStatemachine<object> _statemachine;
        SwordBoosting _swordBoostingState;
        SwordSlash _swordSlashState;

        ISword _sword;

        ArmOccupation _armOccupation;
        public override WeaponType Type => WeaponType.Sword;
        internal TeamMask teamMask
        {
            get => _targetsTrigger.TeamMask;
            set
            {
                _targetsTrigger.TeamMask = value;
            }
        }
        protected override Behaviours.Arms.IArmedWeaponArmBehaviour behaviour
        {
            get
            {
                if (_behaviour == null)
                    throw new Exception("This component node is not initialized, so don't try to get any element of this node.");
                return _behaviour;
            }
        }
        public override bool Activated
        {
            get => base.Activated;
            set
            {
                enabled = value;
                if (_behaviour != null)
                {
                    //UpdateTargetsCatcherFor(blackboard);
                    _behaviour.Activated = value;
                    _targetLocker.Enabled = value;
                }
                Cursor.visible = !value;
                Cursor.lockState = value ? CursorLockMode.Confined : CursorLockMode.None;
            }
        }
        public override IWeapon Weapon
        {
            get => base.Weapon;
            set
            {
                base.Weapon = value;
                if (value is ISword sword)
                {
                    var extensionAction = sword.GetSwordAction(SwordActionType.Extension);
                    var slashAction = sword.GetSwordAction(SwordActionType.Slash);
                    if (_swordBoostingState != null)
                    {
                        _swordBoostingState.ExtensionAction = extensionAction;
                    }
                    if (_swordSlashState != null)
                    {
                        _swordSlashState.ExtensionAction = extensionAction;
                        _swordSlashState.SlashAction = slashAction;
                    }
                    _sword = sword;
                }
            }
        }
        protected override void OnEnable()
        {
            base.OnEnable();

        }
        public override void Update()
        {
            _behaviour?.Update();
        }
        public override void FixedUpdate()
        {
            _behaviour.FixedUpdate();
        }
        //void UpdateTargetsCatcherFor(Blackboard blackboard)
        void CreateSphereTriggerTargetsCatcher(GameObject armObj)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.GetComponent<MeshRenderer>().enabled = false;
            obj.transform.SetParent(armObj.transform, false);
            obj.layer = LayerMask.NameToLayer("Movement");
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            _targetsTrigger = obj.AddComponent<SphericalObjsTrigger>();
            _targetsTrigger.IncludeLayerMask = _definitions.Trigger.IncludeLayerMask;
            _targetsTrigger.ExcludeLayerMask = _definitions.Trigger.ExcludeLayerMask;
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);


            blackboard.TryReadValueOrThrowException<LocomotionCore>(CharacterBlackboardFields.Character_Locomotion_Core, out var locomotionCore);
            blackboard.TryReadValueOrThrowException<IHumanInput>(CharacterBlackboardFields.Character_Input_Main, out var input);
            blackboard.TryReadValueOrThrowException<PlayableGraph>(CharacterBlackboardFields.Character_Animation_Graph, out var graph);
            blackboard.TryReadValueOrThrowException<ControllerPlayable>(CharacterBlackboardFields.Character_Animation_Whole_Body_Animator, out var controller);
            blackboard.TryReadValueOrThrowException<GameObject>(CharacterBlackboardFields.Character_Obj_Arm_Local, out var armObj);
            blackboard.TryReadValueOrThrowException(CharacterBlackboardFields.Character_Component_TargetLocker, out _targetLocker);
            if (blackboard.TryReadValue<TeamMask>(CharacterBlackboardFields.Character_TeamMask, out var teamMask))
            {
                this.teamMask = teamMask;
            }
            else
            {
                throw new BlackboardKeyNotFoundException(CharacterBlackboardFields.Character_TeamMask);
            }

            CreateSphereTriggerTargetsCatcher(armObj);
            _load = new(_targetsTrigger.gameObject);

            InitializeTargetsCatcher(blackboard);

            _definitions.InitializeBy(locomotionCore.definitions);

            var rotationLocker = new RotationLocomotionLocker(locomotionCore.rotation);

            //var boostingHelper = new BoostingHelper(locomotionCore.core, camera, input, _definitions.Boosting);
            var winput = default(IWeaponControlInput);
            if (Part == HumanPart.LeftArm)
                winput = input.LArm?.WeaponControl;
            else if (Part == HumanPart.RightArm)
                winput = input.RArm?.WeaponControl;
            _boostingHelper = new BoostingHelper(locomotionCore.core, _targetLocker, input.BaseInput, winput, _definitions.Boosting);
            _slashHelper = new SlashHelper(locomotionCore.core, rotationLocker, _definitions.Slash.Duration, _definitions.Slash.RecoveryDuration);
            var mixer = InitializeMixer(graph, controller);
            _animator = new(
                graph,
                controller,
                mixer,
                locomotionCore.core,
                locomotionCore.definitions.Walking.MaxSpeed,
                locomotionCore.definitions.Walking.AcceleratedSpeed,
                _boostingHelper,
                _slashHelper,
                _definitions,
                _animationDefinitions);

            _behaviour = new(_definitions, _boostingHelper, _slashHelper, _animator);
            _behaviour.TargetsTrigger = _targetsTrigger;


            _swordBoostingState = new SwordBoosting(_boostingHelper);
            _swordSlashState = new SwordSlash(_slashHelper);

            if (_sword != null)
            {
                var extensionAction = _sword.GetSwordAction(SwordActionType.Extension);
                var slashAction = _sword.GetSwordAction(SwordActionType.Slash);


                _swordBoostingState.ExtensionAction = _swordSlashState.ExtensionAction = extensionAction;
                _swordSlashState.SlashAction = slashAction;
            }
            _swordBoostingState.FollowingSlashState = _swordSlashState;



            var locomotionStatemachine = locomotionCore.statemachine;
            var quickBoostingHelper = locomotionCore.quickBoostingHelper;

            locomotionStatemachine.AddState(_swordBoostingState);
            locomotionStatemachine.AddState(_swordSlashState);


            var sb_m = new BlendingTransition<object>(_swordBoostingState, locomotionCore.movementStatemachine, null, null, 0, 0, 1, InterruptionSource.None);
            var sb_s = new BlendingTransition<object>(_swordBoostingState, _swordSlashState, () => _slashHelper.originalEntryEvent, null, 0, 0, BlendingTransition<object>.FIXED_EXIT_TIME_INVALID_VALUE, InterruptionSource.None);

            locomotionStatemachine.AddTransitionFor(sb_m);
            locomotionStatemachine.AddTransitionFor(sb_s);

            locomotionStatemachine.AddTransitionFor(locomotionCore.movementStatemachine, _swordBoostingState, () => _behaviour.Activated && _boostingHelper.originalEntryEvent);
            locomotionStatemachine.AddTransitionFor(locomotionCore.jump, _swordBoostingState, () => _behaviour.Activated && _boostingHelper.originalEntryEvent);




            locomotionStatemachine.AddTransitionFor(_swordBoostingState, locomotionCore.quickBoosting, () => quickBoostingHelper.TriggerEvent);
            locomotionStatemachine.AddTransitionFor(_swordSlashState, locomotionCore.quickBoosting, () => quickBoostingHelper.TriggerEvent);



            var s_m = new BlendingTransition<object>(_swordSlashState, locomotionCore.movementStatemachine, null, null, 0, 0, 1, InterruptionSource.None);
            locomotionStatemachine.AddTransitionFor(s_m);


            var anotherArmCoreField = GetAnotherArmCoreField();

            if (blackboard.TryReadValue<ArmController>(anotherArmCoreField, out var anotherArmCore))
            {
                WhenAnotherArmEnable(FieldEventType.Writing, null, anotherArmCore);
            }
            else
            {
                blackboard.TryReadValueOrThrowException<FieldChangeHandler>(CharacterBlackboardFields.FieldChangeHandler, out var handler);
                handler.RegisterAction<ArmController>(anotherArmCoreField, WhenAnotherArmEnable);
            }


            this.Activated = this.Activated;

        }
        void WhenAnotherArmEnable(FieldEventType type, ArmController _, ArmController no)
        {
            if (type != FieldEventType.Register && type != FieldEventType.Writing)
                return;
            var core = no;
            if (_statemachine == null)
            {

                var currentField = Part switch
                {
                    HumanPart.None => Guid.Empty,
                    HumanPart.LeftArm => CharacterBlackboardFields.Character_Arm_Left_Core,
                    HumanPart.RightArm => CharacterBlackboardFields.Character_Arm_Right_Core,
                    _ => throw new NotImplementedException()
                };

                blackboard.TryReadValueOrThrowException<ArmController>(currentField, out var currentArmCore);
                var anotherArmCore = core;

                InitializeStatemachine(anotherArmCore, _boostingHelper, _slashHelper);
            }
            else if (no == null)
                _statemachine.Enabled = false;
            else
            {
                var otherArm = _armOccupation.anotherArm;
                otherArm.ArmCore = core;
                _statemachine.Enabled = true;
            }


        }
        /// <summary>
        /// UNDONE: 与另一条手臂的协调逻辑
        /// 问题：
        /// - 过渡时间如何定义 （DONE）
        /// - 还没支持另一条手臂被抢占时，动画的过渡
        /// </summary>
        void InitializeStatemachine(ArmController anotherArmCore, BoostingHelper boostingHelper, SlashHelper slashHelper)
        {
            if (anotherArmCore == null)
                return;
            _statemachine = new("arm_occupation");

            var anotherArm = new AnotherArm(anotherArmCore, "another_arm");
            var currentArm = new CurrentArm(anotherArmCore, "current_arm");

            _statemachine.AddState(anotherArm);
            _statemachine.AddState(currentArm);

            var oa_c = new BlendingTransition<object>(anotherArm, currentArm, () => _behaviour.Activated && boostingHelper.EntryEvent, null, _animationDefinitions.GetTransitionOptions(Transition.Idle_Boosting));
            _statemachine.AddTransitionFor(oa_c);

            var c_oa_s = new BlendingTransition<object>(currentArm, anotherArm, () => _behaviour.Activated && slashHelper.ExitEvent, null, _animationDefinitions.GetTransitionOptions(Transition.Slash_Idle));
            var c_oa_b = new BlendingTransition<object>(currentArm, anotherArm, () => _behaviour.Activated && !slashHelper.EntryEvent && boostingHelper.ExitEvent, null, _animationDefinitions.GetTransitionOptions(Transition.Boosting_Idle));
            _statemachine.AddTransitionFor(c_oa_b);
            _statemachine.AddTransitionFor(c_oa_s);

            _armOccupation = new() { currentArm = currentArm, anotherArm = anotherArm };

        }
        WholeBodyMixerPlayable InitializeMixer(PlayableGraph graph, ControllerPlayable baseController)
        {
            var bnode = baseController.Node;
            var parent = bnode.Parent as IAnimationPlayablePartNode;
            if (parent == null)
                throw new NullReferenceException(nameof(parent));
            if (parent?.Value is WholeBodyMixerPlayable mixer)
                return mixer;
            else
            {
                parent.RemoveChild(bnode);

                mixer = new WholeBodyMixerPlayable(graph, 3);
                mixer.OutputSetting.Weight = 1;

                parent.AddChild(mixer.Node);

                mixer.Node.AddChild(bnode);
                return mixer;
            }
        }
        void InitializeTargetsCatcher(Blackboard blackboard)
        {
            var field = Part switch
            {
                HumanPart.LeftArm => MountPointFields.Left_Chest_Trigger,
                HumanPart.RightArm => MountPointFields.Right_Chest_Trigger,
                _ => throw new Exception(),
            };
            blackboard.TryGetMountPointOrThrowException(field, out var mountPoint);
            mountPoint.Load = _load;
        }
        Guid GetAnotherArmCoreField() => Part switch
        {
            HumanPart.None => Guid.Empty,
            HumanPart.LeftArm => CharacterBlackboardFields.Character_Arm_Right_Core,
            HumanPart.RightArm => CharacterBlackboardFields.Character_Arm_Left_Core,
            _ => throw new NotImplementedException()
        };
        public override void Dispose()
        {
            var field = Part switch
            {
                HumanPart.LeftArm => MountPointFields.Left_Chest_Trigger,
                HumanPart.RightArm => MountPointFields.Right_Chest_Trigger,
                _ => throw new Exception(),
            };
            base.Dispose();
            //blackboard.TryUnregisterField(CharacterBlackboardFields.TargetLocker);
            blackboard.TryGetMountPointOrThrowException(field, out var mountPoint);

            blackboard.TryReadValueOrThrowException<FieldChangeHandler>(CharacterBlackboardFields.FieldChangeHandler, out var handler);
            field = GetAnotherArmCoreField();
            handler.UnregisterAction<ArmController>(field, WhenAnotherArmEnable);

            mountPoint.Load = null;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _statemachine?.OnEnter();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            _statemachine?.OnUpdate();
        }

        public override void OnExit()
        {
            _statemachine?.OnExit();
            base.OnExit();
        }

        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            _statemachine?.ChangeStateTo(_armOccupation.anotherArm);
            _statemachine?.FromPreviousStateTransitionBegin(currentTransition);
        }

        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            _statemachine?.FromPreviousStateTransitionRunning(currentTransition);
        }

        public override void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionEnd(currentTransition);
            _statemachine?.FromPreviousStateTransitionEnd(currentTransition);
        }

        public override void ToNextStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionBegin(currentTransition);
            _statemachine?.ToNextStateTransitionBegin(currentTransition);
        }

        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            _statemachine?.ToNextStateTransitionRunning(currentTransition);
        }

        public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionEnd(currentTransition);
            _statemachine?.ToNextStateTransitionEnd(currentTransition);
        }

    }
}
