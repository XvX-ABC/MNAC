using System;
using MNAC.Animations;
using MNAC.Behaviours;
using MNAC.Behaviours.Arms.Weapons.Sword.Animations;
using MNAC.Characters.Humanoid.Input;
using MNAC.Behaviours.Input;
using MNAC.Characters.MountPoints;
using MNAC.Characters.UI;
using MNAC.Interaction;
using MNAC.States;
using MNAC.UI;
using MNAC.Utilities.Blackboards;
using MNAC.Utilities.MountPoints;
using MNAC.Utilities.Timeline;
using MNAC.Utilities.Timeline.Events;
using MNAC.Utilities.Timeline.Events.Range;
using MNAC.Weapons;
using MNAC.Weapons.Sword;
using UnityEngine;
using UnityEngine.Playables;
using LocomotionCore = MNAC.Characters.Humanoid.Locomotion.LocomotionCore;
using PlayerCursorIndicator = MNAC.Characters.UI.PlayerCursorIndicator;
using SphericalObjsTrigger = MNAC.Characters.Interaction.SphericalObjsTrigger;
using Transition = MNAC.Behaviours.Arms.Weapons.Sword.Animations.IArmedSwordArmAnimationDefinitions.Transition;
namespace MNAC.Characters.Humanoid.Arms.Weapons.Sword
{
    [CreateAssetMenu(fileName = "ArmedSwordArmBehaviour", menuName = "Tests/Behaviours/Characters/Humanoid/Arms/Weapons/Sword/ArmedSwordArmBehaviour")]
    public class ArmedSwordArmBehaviour_SO : ArmedArmBehaviourBase_SO
    {
        #region internal classes
        class UIControl
        {
            const string SLIDER_MODE_RELOAD = "filling";
            const string SLIDER_MODE_NORMALLY = "normal";
            ITimeline _reloadTimeline;
            ITimelineEvent _reloadingEvent;

            PlayerCursorIndicator _indicator;
            TextGrid _textGrid;
            TextBox _textBox;
            ProgressSlider _slider;
            HumanBodyPart _part;

            public UIControl(HumanBodyPart part, ITimeline cdTimeline)
            {
                _part = part;
                reloadTimeline = cdTimeline ?? throw new ArgumentNullException(nameof(cdTimeline));
            }
            internal PlayerCursorIndicator cursorIndicator
            {
                get => _indicator;
                set
                {
                    _indicator = value;
                    if (_indicator != null)
                    {

                        _slider = _part switch
                        {
                            HumanBodyPart.LeftArm => _indicator.Slider_lb,
                            HumanBodyPart.RightArm => _indicator.Slider_rb,
                            _ => null
                        };
                        _slider.ChangeMode(SLIDER_MODE_NORMALLY);
                    }
                    else
                        _slider = null;
                }
            }

            internal TextGrid textGrid
            {
                get => _textGrid;
                set
                {
                    TextBoxDispose();
                    if (value != null)
                    {
                        _textBox = _part switch
                        {
                            HumanBodyPart.LeftArm => value.TextBox_2,
                            HumanBodyPart.RightArm => value.TextBox_3,
                            _ => null
                        };
                    }
                    _textGrid = value;
                }
            }

            internal ITimeline reloadTimeline
            {
                get => _reloadTimeline;
                set
                {
                    if (value != null)
                    {
                        value.StartAction += WhenReloadStart;
                        _reloadingEvent = value.AddRangeEvent(0, 1, WhenReloading);
                        value.EndAction += WhenReloadEnd;
                    }
                    TimelineDispose();
                    _reloadTimeline = value;
                }
            }


            public void Load()
            {
                _slider.Value = reloadTimeline.NormalizedTime;
            }
            public void Reset()
            {
                _slider.Value = 1;
                _textBox.Text = "";
            }
            public void Dispose()
            {
                TimelineDispose();
                TextBoxDispose();
            }
            void TimelineDispose()
            {
                if (_reloadTimeline != null)
                {
                    _reloadTimeline.StartAction -= WhenReloadStart;
                    if (_reloadingEvent != null)
                        _reloadTimeline.RemoveRangeEvent(_reloadingEvent);
                    _reloadTimeline.EndAction -= WhenReloadEnd;
                }
            }
            void TextBoxDispose()
            {
                if (_textBox != null)
                {
                    _textBox.Text = "";
                }
            }
            void WhenReloadStart(TimelineContext _)
            {
                _slider?.ChangeMode(SLIDER_MODE_RELOAD);
            }
            void WhenReloading(TimelineContext ctx)
            {
                if (_slider != null)
                {
                    _slider.Value = ctx.NormalizedTime;
                }
            }
            void WhenReloadEnd(TimelineContext _)
            {
                _slider?.ChangeMode(SLIDER_MODE_NORMALLY);
            }
            ~UIControl()
            {
                Dispose();
            }
        }
        #endregion
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

        internal BoostingHelper boostingHelper;
        internal SlashHelper slashHelper;

        WithCallbackPlayableStatemachine<object> _occupationStatemachine;
        SwordBoosting _swordBoostingState;
        SwordSlash _swordSlashState;

        ISword _sword;

        TeamMask _teamMask;

        ArmsOccupation _armsOccupation;


        UIControl _uiControl;
        public override WeaponType Type => WeaponType.Sword;
        internal TeamMask teamMask
        {
            get => _targetsTrigger.TeamMask;
            set
            {
                if (_sword != null)
                    _sword.TeamMask = value;
                _targetsTrigger.TeamMask = value;
            }
        }
        protected override Behaviours.Arms.IArmedArmBehaviour behaviour
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
                    //_targetLocker.Enabled = value;
                }
                //Cursor.visible = !value;
                //Cursor.lockState = value ? CursorLockMode.Confined : CursorLockMode.None;
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

                    if (blackboard.TryReadValue<HumanoidBodyParts>(CharacterBlackboardFields.Character_BodyParts, out var bodyParts))
                    {
                        var chestObj = bodyParts.GetItem((uint)HumanBodyPart.Chest);
                        sword.OwnerObj = chestObj;
                    }


                    sword.TeamMask = _teamMask;
                    _sword = sword;
                }
            }
        }


        protected override void OnEnable()
        {
            base.OnEnable();

        }
        public override void BehaviourOnUpdate()
        {
            _behaviour?.BehaviourOnUpdate();
        }
        public override void BehaviourOnFixedUpdate()
        {
            _behaviour.BehaviourOnFixedUpdate();
        }
        //void UpdateTargetsCatcherFor(Blackboard blackboard)
        void CreateSphereTriggerTargetsCatcher(LocomotionCore locomotionCore, GameObject armObj)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.GetComponent<MeshRenderer>().enabled = false;
            obj.transform.SetParent(armObj.transform, false);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            _targetsTrigger = obj.AddComponent<SphericalObjsTrigger>();
            _targetsTrigger.Initialize(locomotionCore.internalCore, 45);
            _targetsTrigger.IncludeLayerMask = _definitions.Trigger.IncludeLayerMask;
            _targetsTrigger.ExcludeLayerMask = _definitions.Trigger.ExcludeLayerMask;
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);


            blackboard.TryReadValueOrThrowException<LocomotionCore>(CharacterBlackboardFields.Character_Locomotion_Core, out var locomotionCore);
            blackboard.TryReadValueOrThrowException<IHumanoidInput>(CharacterBlackboardFields.Character_Input_Main, out var input);
            blackboard.TryReadValueOrThrowException<PlayableGraph>(CharacterBlackboardFields.Character_Animation_Graph, out var graph);
            blackboard.TryReadValueOrThrowException<ControllerPlayable>(CharacterBlackboardFields.Character_Animation_Whole_Body_Animator, out var controller);
            blackboard.TryReadValueOrThrowException<GameObject>(CharacterBlackboardFields.Character_Obj_Arm_Local, out var armObj);
            blackboard.TryReadValueOrThrowException(CharacterBlackboardFields.Character_Components_TargetLocker, out _targetLocker);



            CreateSphereTriggerTargetsCatcher(locomotionCore, armObj);
            _load = new(_targetsTrigger.gameObject);

            if (blackboard.TryReadValue<TeamMask>(CharacterBlackboardFields.Character_TeamMask, out var teamMask))
            {
                this.teamMask = teamMask;
            }
            else
            {
                throw new BlackboardKeyNotFoundException(CharacterBlackboardFields.Character_TeamMask);
            }



            InitializeTargetsCatcher(blackboard);

            _definitions.InitializeBy(locomotionCore.definitions);

            var rotationLocker = new RotationLocomotionLocker(locomotionCore.rotationModule);

            var winput = default(IWeaponControlInput);
            if (Part == HumanBodyPart.LeftArm)
                winput = input.LArm?.WeaponControl;
            else if (Part == HumanBodyPart.RightArm)
                winput = input.RArm?.WeaponControl;
            boostingHelper = new BoostingHelper(locomotionCore.internalCore, rotationLocker, _targetLocker, input.BaseInput, winput, _definitions.Boosting);
            slashHelper = new SlashHelper(locomotionCore.internalCore, rotationLocker, _definitions.Slash.Duration, _definitions.Slash.RecoveryDuration);
            var mixer = InitializeMixer(graph, controller);
            _animator = new(
                Part,
                graph,
                controller,
                mixer,
                locomotionCore.internalCore,
                locomotionCore.definitions.Walking.MaxSpeed,
                locomotionCore.definitions.Walking.AcceleratedSpeed,
                boostingHelper,
                slashHelper,
                _definitions,
                _animationDefinitions);

            _behaviour = new(_definitions, boostingHelper, slashHelper, _animator);
            _behaviour.TargetsTrigger = _targetsTrigger;

            if (blackboard.TryReadValue<LayerMask>(CharacterBlackboardFields.Character_Weapon_Sword_LayerMaskToHit, out var layerMask))
            {
                _behaviour.LayerMaskToHit = layerMask;
                _targetsTrigger.IncludeLayerMask |= layerMask;
            }
            else
                throw new BlackboardKeyNotFoundException(CharacterBlackboardFields.Character_Weapon_Sword_LayerMaskToHit);


            _swordBoostingState = new SwordBoosting(boostingHelper);
            _swordSlashState = new SwordSlash(slashHelper);

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
            var sb_s = new BlendingTransition<object>(_swordBoostingState, _swordSlashState, () => slashHelper.originalEntryEvent, null, 0, 0, BlendingTransition<object>.FIXED_EXIT_TIME_INVALID_VALUE, InterruptionSource.None);

            locomotionStatemachine.AddTransitionFor(sb_m);
            locomotionStatemachine.AddTransitionFor(sb_s);

            locomotionStatemachine.AddTransitionFor(locomotionCore.movementStatemachine, _swordBoostingState, () => _behaviour.Activated && boostingHelper.originalEntryEvent);
            locomotionStatemachine.AddTransitionFor(locomotionCore.jump, _swordBoostingState, () => _behaviour.Activated && boostingHelper.originalEntryEvent);




            locomotionStatemachine.AddTransitionFor(_swordBoostingState, locomotionCore.quickBoosting, () => quickBoostingHelper.TriggerEvent);
            locomotionStatemachine.AddTransitionFor(_swordSlashState, locomotionCore.quickBoosting, () => quickBoostingHelper.TriggerEvent);



            var s_m = new BlendingTransition<object>(_swordSlashState, locomotionCore.movementStatemachine, null, null, 0, 0, 1, InterruptionSource.None);
            locomotionStatemachine.AddTransitionFor(s_m);


            var anotherArmControllerField = GetAnotherArmCoreField();

            //if (blackboard.TryReadValue<ArmController>(anotherArmControllerField, out var anotherArmCore))
            //{
            //    WhenAnotherArmEnable(FieldEventType.Writing, null, anotherArmCore);
            //}
            //else
            //{
            //blackboard.TryReadValueOrThrowException<FieldChangeHandler>(CharacterBlackboardFields.FieldChangeHandler, out var handler);
            //handler.RegisterAction_Obsolete<ArmController>(anotherArmCoreField, WhenAnotherArmEnable);
            GetAnotherArmController(anotherArmControllerField);
            //}


            if (blackboard.TryReadUIValue<PlayerCursorIndicator>(CharacterUIBlackboardFields.Player_Cursor_Indicator, out var indicator)
                && blackboard.TryReadUIValue<TextGrid>(CharacterUIBlackboardFields.Weapons_Text_Grid, out var textGrid))
            {
                _uiControl = new(Part, boostingHelper.cdTimeline);
                _uiControl.cursorIndicator = indicator;
                _uiControl.textGrid = textGrid;
            }

            this.Activated = this.Activated;

        }
        void GetAnotherArmController(Guid fieldID)
        {
            if (blackboard.TryReadValue<ArmController>(fieldID, out var anotherController))
            {
                WhenAnotherArmEnable(FieldEventType.Writing, null, anotherController);
            }
            blackboard.RegisterFieldChangeAction<ArmController>(fieldID, WhenAnotherArmEnable);
        }
        void WhenAnotherArmEnable(FieldEventType type, ArmController _, ArmController no)
        {
            if (type != FieldEventType.Register && type != FieldEventType.Writing)
                return;
            var core = no;
            if (_occupationStatemachine == null)
            {

                var currentField = Part switch
                {
                    HumanBodyPart.None => Guid.Empty,
                    HumanBodyPart.LeftArm => CharacterBlackboardFields.Character_Arm_Left_Controller,
                    HumanBodyPart.RightArm => CharacterBlackboardFields.Character_Arm_Right_Controller,
                    _ => throw new NotImplementedException()
                };

                blackboard.TryReadValueOrThrowException<ArmController>(currentField, out var currentArmCore);
                var anotherArmCore = core;

                InitializeStatemachine(anotherArmCore, boostingHelper, slashHelper);
            }
            else if (no == null)
                _occupationStatemachine.Enabled = false;
            else
            {
                var unoccupited = _armsOccupation.unoccupited;
                unoccupited.ArmController = core;
                _occupationStatemachine.Enabled = true;
            }


        }
        void InitializeStatemachine(ArmController anotherArmController, BoostingHelper boostingHelper, SlashHelper slashHelper)
        {
            if (anotherArmController == null)
                return;
            _occupationStatemachine = new("arm_occupation");

            var unoccupited = new Unoccupied(anotherArmController, "unoccupited");

            var occupited = new Occupited(anotherArmController, "occupited");


            _occupationStatemachine.AddState(unoccupited);
            _occupationStatemachine.AddState(occupited);

            var oa_c = new BlendingTransition<object>(unoccupited, occupited, () => _behaviour.Activated && boostingHelper.EntryEvent, null, _animationDefinitions.GetTransitionOptions(Transition.Idle_Boosting));
            _occupationStatemachine.AddTransitionFor(oa_c);

            var c_oa_s = new BlendingTransition<object>(occupited, unoccupited, () => _behaviour.Activated && slashHelper.ExitEvent, null, _animationDefinitions.GetTransitionOptions(Transition.Slash_Idle));
            var c_oa_b = new BlendingTransition<object>(occupited, unoccupited, () => _behaviour.Activated && !slashHelper.EntryEvent && boostingHelper.ExitEvent, null, _animationDefinitions.GetTransitionOptions(Transition.Boosting_Idle));
            _occupationStatemachine.AddTransitionFor(c_oa_b);
            _occupationStatemachine.AddTransitionFor(c_oa_s);

            _armsOccupation = new() { occupited = occupited, unoccupited = unoccupited };

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
                HumanBodyPart.LeftArm => MountPointFields.Left_Chest_Trigger,
                HumanBodyPart.RightArm => MountPointFields.Right_Chest_Trigger,
                _ => throw new Exception(),
            };
            blackboard.TryGetMountPointOrThrowException(field, out var mountPoint);
            mountPoint.Load = _load;
        }
        Guid GetAnotherArmCoreField() => Part switch
        {
            HumanBodyPart.None => Guid.Empty,
            HumanBodyPart.LeftArm => CharacterBlackboardFields.Character_Arm_Right_Controller,
            HumanBodyPart.RightArm => CharacterBlackboardFields.Character_Arm_Left_Controller,
            _ => throw new NotImplementedException()
        };
        public override void Dispose()
        {

            var field = Part switch
            {
                HumanBodyPart.LeftArm => MountPointFields.Left_Chest_Trigger,
                HumanBodyPart.RightArm => MountPointFields.Right_Chest_Trigger,
                _ => throw new Exception(),
            };
            blackboard.TryGetMountPointOrThrowException(field, out var mountPoint);

            field = GetAnotherArmCoreField();
            blackboard.UnregisterFieldChangeAction<ArmController>(field, WhenAnotherArmEnable);

            mountPoint.Load = null;
            if (_uiControl != null)
                _uiControl.Dispose();
            base.Dispose();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _occupationStatemachine?.OnEnter();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            _occupationStatemachine?.OnUpdate();
            //Debug.Log(_occupationStatemachine);
        }

        public override void OnExit()
        {
            _occupationStatemachine?.OnExit();
            base.OnExit();
        }

        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            _occupationStatemachine?.ChangeStateTo(_armsOccupation.unoccupited);

            //_occupationStatemachine?.FromPreviousStateTransitionBegin(currentTransition);
        }

        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            //_occupationStatemachine?.FromPreviousStateTransitionRunning(currentTransition);
        }

        public override void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionEnd(currentTransition);
            //_occupationStatemachine?.FromPreviousStateTransitionEnd(currentTransition);
        }

        public override void ToNextStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionBegin(currentTransition);
            //_occupationStatemachine?.ToNextStateTransitionBegin(currentTransition);
        }

        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            //_occupationStatemachine?.ToNextStateTransitionRunning(currentTransition);
        }

        public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionEnd(currentTransition);
            //_occupationStatemachine?.ToNextStateTransitionEnd(currentTransition);
        }

    }
}
