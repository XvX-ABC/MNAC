using RootMotion.FinalIK;
using System;
using System.Text;
using MNAC.Behaviours;
using MNAC.Behaviours.Arms.Weapons;
using MNAC.Behaviours.Arms.Weapons.Launcher.Animations;
using MNAC.Behaviours.Input;
using MNAC.Characters.Humanoid.Input;
using MNAC.Characters.Humanoid.Locomotion;
using MNAC.Characters.Interaction.Input;
using MNAC.Characters.UI;
using MNAC.Interaction;
using MNAC.TPhysics;
using MNAC.TPhysics.Environment;
using MNAC.UI;
using MNAC.Utilities.Blackboards;
using MNAC.Utilities.Timeline;
using MNAC.Utilities.Timeline.Events;
using MNAC.Utilities.Timeline.Events.Range;
using MNAC.Weapons;
using MNAC.Weapons.Launcher;
using UnityEngine;
using UnityEngine.Playables;
using PlayerCursorIndicator = MNAC.UI.PlayerCursorIndicator;
using WeaponType = MNAC.Weapons.WeaponType;

namespace MNAC.Characters.Humanoid.Arms.Weapons.Launchers
{
    [CreateAssetMenu(fileName = "ArmedLauncherArmBehaviour", menuName = SOHelper.BEHAVIOURS_MENU_NAME + "/ArmedLauncherArmBehaviour")]
    public class ArmedLauncherArmBehaviour_SO : ArmedArmBehaviourBase_SO
    {
        #region internal classes
        class UIControl : IDisposable
        {
            const string SLIDER_MODE_RELOAD = "filling";
            const string SLIDER_MODE_NORMALLY = "normal";
            ILauncher _launcher;
            float _maximumMagazineAmount;
            float _maximumReserveAmount;
            float _nextReloadAmount;
            ITimeline _reloadTimeline;
            ITimelineEvent _reloadingEvent;

            PlayerCursorIndicator _indicator;
            TextGrid _textGrid;
            TextBox _textBox;
            ProgressSlider _slider;
            HumanBodyPart _part;

            public UIControl(HumanBodyPart part)
            {
                _part = part;
            }
            internal ITimeline reloadTimeline
            {
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
            internal ILauncher launcher
            {
                get => _launcher;
                set
                {
                    if (value != null)
                    {
                        value.MagazineAmountChangeAction += WhenMagazineAmountChange;
                        value.ReserveAmountChangeAction += WhenReserveAmountChange;
                        _maximumMagazineAmount = value.Definitions.AmmoInMagazineAmount;
                        _maximumReserveAmount = value.Definitions.AmmoReserveAmount;

                        reloadTimeline = value.ReloadTimeline;
                    }

                    LauncherDispose();
                    _launcher = value;
                }
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
                        _textBox.Text = _maximumMagazineAmount.ToString();
                    }
                    _textGrid = value;
                }
            }

            void LauncherDispose()
            {
                if (_launcher != null)
                {
                    _launcher.MagazineAmountChangeAction -= WhenMagazineAmountChange;
                    _launcher.ReserveAmountChangeAction -= WhenReserveAmountChange;
                }
            }
            void TimelineDispose()
            {
                if (_reloadTimeline != null)
                {
                    _reloadTimeline.StartAction -= WhenReloadStart;
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
            public void Load()
            {
                if (_launcher == null)
                    return;
                _slider.Value = _launcher.MagazineAmmoAmount / _maximumMagazineAmount;
                _textBox.Text = _maximumReserveAmount.ToString();
            }
            public void Reset()
            {
                _slider.Value = 1;
                _textBox.Text = "";
            }
            public void Dispose()
            {
                LauncherDispose();
                TimelineDispose();
                TextBoxDispose();
            }

            void WhenMagazineAmountChange(int ov, int nv)
            {
                if (_slider == null)
                {
                    return;
                }
                var t = 0f;
                if (nv > 0)
                    t = (float)nv / _maximumMagazineAmount;
                _slider.Value = t;
            }
            void WhenReserveAmountChange(int ov, int nv)
            {
                _textBox.Text = nv.ToString();
            }
            void WhenReloadStart(TimelineContext _)
            {
                _slider?.ChangeMode(SLIDER_MODE_RELOAD);
                _nextReloadAmount = Mathf.Min(_launcher.ReserveAmmoAmount, _maximumMagazineAmount);
            }
            void WhenReloading(TimelineContext ctx)
            {
                if (_slider != null)
                {
                    var v = _nextReloadAmount / _maximumMagazineAmount;
                    _slider.Value = Mathf.Lerp(0, v, ctx.NormalizedTime);
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

        GameObject _characterObj;
        ArmedLauncherArmAnimator _animator;
        Behaviours.Arms.Weapons.Launcher.ArmedLauncherArmBehaviour _behaviour;
        [SerializeField]
        ArmedLauncherArmBehavioursDefinitions_SO _definitions;
        [SerializeField]
        ArmedLauncherArmAnimationDefinitions_SO _animationDefinitions;
        ITargetLocker _targetLocker;
        UIControl _uiControl;
        public override WeaponType Type => WeaponType.Launcher;

        public override IWeapon Weapon
        {
            get => _behaviour.Weapon;
            set
            {
                _behaviour.Weapon = value;
                if (value is ILauncher launcher)
                {
                    if (_uiControl != null)
                        _uiControl.launcher = launcher;
                }
            }
        }

        public override IArmedArmAnimationPlayablePart Animator => _behaviour.Animator;

        public override Func<bool> ActivationTrigger => _behaviour.ActivationTrigger;

        public override Func<bool> UnactivationTrigger => _behaviour.UnactivationTrigger;

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
                if (_behaviour != null)
                {
                    _sb.Append(_characterObj.name);
                    _sb.Append(" ( ");
                    _sb.Append(this.GetType().Name);
                    _sb.Append(" ) : ");
                    _sb.Append(" The behaviour's activated state changes to ");
                    _sb.Append($"'{value}'");
                    //Debug.Log(_sb.ToString());
                    _sb.Clear();
                    _behaviour.Activated = value;
                    //_targetLocker.Enabled = value;

                }
                if (_uiControl != null)
                {
                    if (value)
                        _uiControl.Load();
                    else
                        _uiControl.Reset();
                }
                enabled = value;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);

            blackboard.TryReadValueOrThrowException<Rigidbody>(CharacterBlackboardFields.Rigidbody, out var rbody);
            blackboard.TryReadValueOrThrowException<World>(CharacterBlackboardFields.World, out var world);
            blackboard.TryReadValueOrThrowException<IGroundDetector>(CharacterBlackboardFields.GroundDetector, out var groundDetector);
            blackboard.TryReadValueOrThrowException<LocomotionCore>(CharacterBlackboardFields.Character_Locomotion_Core, out var locomotionCore);
            blackboard.TryReadValueOrThrowException<PlayableGraph>(CharacterBlackboardFields.Character_Animation_Graph, out var graph);
            blackboard.TryReadValueOrThrowException<GameObject>(CharacterBlackboardFields.Character_Obj_Main, out _characterObj);
            blackboard.TryReadValueOrThrowException<GameObject>(CharacterBlackboardFields.Character_Obj_Arm_Local, out var armObj);

            blackboard.TryReadValueOrThrowException<IHumanoidInput>(CharacterBlackboardFields.Character_Input_Main, out var input);



            var aimIK = armObj.GetComponent<AimIK>();

            var armInput = Part switch
            {
                HumanBodyPart.LeftArm => input.LArm,
                HumanBodyPart.RightArm => input.RArm,
                _ => throw new Exception("The body part must be one of the arms.")
            };


            var weaponControlInput = armInput.WeaponControl;
            blackboard.TryReadValueOrThrowException(CharacterBlackboardFields.Character_Components_TargetLocker, out _targetLocker);
            InitializeBehaviourAndAnimator(graph, aimIK, input.BaseInput, rbody, world, groundDetector, _targetLocker, locomotionCore, armInput.WeaponControl);


        }
        void InitializeBehaviourAndAnimator(
            PlayableGraph graph,
            AimIK aimIK,
            IBaseInput baseInput,
            Rigidbody rbody,
            World world,
            IGroundDetector groundDetector,
            ITargetLocker targetLocker,
            LocomotionCore locomotionCore,
            IWeaponControlInput weaponControlInput)
        {
            var targetChangeDuration = _definitions.TargetInteraction.SwitchDuration;
            targetLocker.TargetChangeDuration = targetChangeDuration;
            _behaviour = new(Part, _definitions);
            _behaviour.animator = _animator = new(
                _behaviour,
                graph,
                aimIK,
                rbody,
                world,
                groundDetector,
                locomotionCore,
                targetChangeDuration,
                _definitions,
                _animationDefinitions,
                weaponControlInput);
            _behaviour.InitializeStatemachine();


            if (blackboard.TryReadValue<LayerMask>(CharacterBlackboardFields.Character_Weapon_Projectile_LayerMaskToHit, out var layerMask))
            {
                _behaviour.layerMaskToHit = layerMask;
            }
            if (blackboard.TryReadValue<TeamMask>(CharacterBlackboardFields.Character_TeamMask, out var teamMask))
            {
                _behaviour.teamMask = teamMask;
            }
            if (blackboard.TryReadUIValue<PlayerCursorIndicator>(CharacterUIBlackboardFields.Player_Cursor_Indicator, out var indicator)
                && blackboard.TryReadUIValue<TextGrid>(CharacterUIBlackboardFields.Weapons_Text_Grid, out var textGrid))
            {
                _uiControl = new(Part);
                _uiControl.cursorIndicator = indicator;
                _uiControl.textGrid = textGrid;
            }



            _behaviour.Input = weaponControlInput;
            _behaviour.TargetLocker = _targetLocker;
        }
        public override void Dispose()
        {
            _uiControl?.Dispose();
            base.Dispose();
        }
        public override void BehaviourOnUpdate()
        {
            _behaviour.BehaviourOnUpdate();
            //Cursor.lockState = _targetLocker.MainLockTarget == null ? CursorLockMode.None : CursorLockMode.Locked;
        }
        StringBuilder _sb = new StringBuilder();
        public override void BehaviourOnFixedUpdate()
        {
            var statemachine = _behaviour.statemachine;
            var animationStatemachine = _behaviour.animator.statemachine;
            _sb.Append(_characterObj.name);
            _sb.Append(" ( ");
            _sb.Append(this.GetType().Name);
            _sb.Append(" ) : \n");
            _sb.Append(statemachine);
            _sb.Append(animationStatemachine);
            //Debug.Log(_sb.ToString());
            _sb.Clear();
        }
    }
}
