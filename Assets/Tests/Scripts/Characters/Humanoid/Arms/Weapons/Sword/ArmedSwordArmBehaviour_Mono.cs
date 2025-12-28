using System;
using Tests.Animations;
using Tests.Behaviours;
using Tests.Behaviours.Arms.Weapons.Sword.Animations;
using Tests.Characters.Humanoid.Input;
using Tests.Characters.Interaction.Input;
using Tests.Characters.MountPoints;
using Tests.Characters.UI;
using Tests.Interaction;
using Tests.States;
using Tests.UI;
using Tests.Utilities.Blackboards;
using Tests.Utilities.MountPoints;
using Tests.Utilities.Timeline;
using Tests.Utilities.Timeline.Events;
using Tests.Utilities.Timeline.Events.Range;
using Tests.Weapons_New;
using Tests.Weapons_New.Sword;
using UnityEngine;
using UnityEngine.Playables;
using LocomotionCore = Tests.Characters.Humanoid.Locomotion.LocomotionCore;
using PlayerCursorIndicator = Tests.Characters.UI.PlayerCursorIndicator;
using SphericalObjsTrigger = Tests.Characters.Interaction.SphericalObjsTrigger;
using Transition = Tests.Behaviours.Arms.Weapons.Sword.Animations.IArmedSwordArmAnimationDefinitions.Transition;
namespace Tests.Characters.Humanoid.Arms.Weapons.Sword
{
    public class ArmedSwordArmBehaviour_Mono : ArmedWeaponArmBehaviourBase_Mono
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

        WithCallbackPlayableStatemachine<object> _statemachine;
        SwordBoosting _swordBoostingState;
        SwordSlash _swordSlashState;

        ISword _sword;

        TeamMask _teamMask;

        ArmOccupation _armOccupation;


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
                    sword.TeamMask = _teamMask;
                    _sword = sword;
                }
            }
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
            blackboard.TryReadValueOrThrowException(CharacterBlackboardFields.Character_Component_TargetLocker, out _targetLocker);



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

            //var boostingHelper = new BoostingHelper(locomotionCore.core, camera, input, _definitions.Boosting);
            var winput = default(IWeaponControlInput);
            if (Part == HumanBodyPart.LeftArm)
                winput = input.LArm?.WeaponControl;
            else if (Part == HumanBodyPart.RightArm)
                winput = input.RArm?.WeaponControl;
            boostingHelper = new BoostingHelper(locomotionCore.internalCore, _targetLocker, input.BaseInput, winput, _definitions.Boosting);
            slashHelper = new SlashHelper(locomotionCore.internalCore, rotationLocker, _definitions.Slash.Duration, _definitions.Slash.RecoveryDuration);
            var mixer = InitializeMixer(graph, controller);
            _animator = new(
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


            if (blackboard.TryReadUIValue<PlayerCursorIndicator>(CharacterUIBlackboardFields.Player_Cursor_Indicator, out var indicator)
                && blackboard.TryReadUIValue<TextGrid>(CharacterUIBlackboardFields.Weapons_Text_Grid, out var textGrid))
            {
                _uiControl = new(Part, boostingHelper.cdTimeline);
                _uiControl.cursorIndicator = indicator;
                _uiControl.textGrid = textGrid;
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
            _uiControl.Dispose();
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
