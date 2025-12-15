using RootMotion.FinalIK;
using System;
using Tests.Behaviours;
using Tests.Behaviours.Arms.Weapons;
using Tests.Behaviours.Arms.Weapons.Launcher.Animations;
using Tests.Behaviours.Input;
using Tests.Characters.Humanoid.Interaction.Input;
using Tests.Characters.Humanoid.Locomotion;
using Tests.Characters.Interaction.Input;
using Tests.Characters.UI;
using Tests.Interaction;
using Tests.TPhysics;
using Tests.TPhysics.Environment;
using Tests.UI;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Timeline;
using Tests.Utilities.Timeline.Events;
using Tests.Utilities.Timeline.Events.Range;
using Tests.Weapons_New;
using Tests.Weapons_New.Launcher;
using UnityEngine;
using UnityEngine.Playables;
using PlayerCursorIndicator = Tests.UI.PlayerCursorIndicator;
using WeaponType = Tests.Weapons_New.WeaponType;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    [CreateAssetMenu(fileName = "ArmedLauncherArmBehaviour", menuName = "Tests/Behaviours/Characters/Humanoid/Arms/Weapons/Launchers/ArmedLauncherArmBehaviour")]
    public class ArmedLauncherArmBehaviour_SO : ArmedWeaponArmBehaviourBase_SO
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
            HumanPart _part;

            public UIControl(HumanPart part)
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
                            HumanPart.LeftArm => _indicator.Slider_lb,
                            HumanPart.RightArm => _indicator.Slider_rb,
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
                            HumanPart.LeftArm => value.TextBox_2,
                            HumanPart.RightArm => value.TextBox_3,
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

        public override IArmedWeaponArmAnimationPlayablePart Animator => _behaviour.Animator;

        public override Func<bool> EntryFunc => _behaviour.EntryFunc;

        public override Func<bool> ExitFunc => _behaviour.ExitFunc;

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
                if (_behaviour != null)
                {
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
                Cursor.visible = !value;
                Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
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
            blackboard.TryReadValueOrThrowException<GameObject>(CharacterBlackboardFields.Character_Obj_Main, out var actorObj);
            blackboard.TryReadValueOrThrowException<GameObject>(CharacterBlackboardFields.Character_Obj_Arm_Local, out var armObj);

            blackboard.TryReadValueOrThrowException<IHumanInput>(CharacterBlackboardFields.Character_Input_Main, out var input);



            var aimIK = armObj.GetComponent<AimIK>();

            var armInput = Part switch
            {
                HumanPart.LeftArm => input.LArm,
                HumanPart.RightArm => input.RArm,
                _ => null
            };


            var weaponControlInput = armInput.WeaponControl;
            blackboard.TryReadValueOrThrowException(CharacterBlackboardFields.Character_Component_TargetLocker, out _targetLocker);
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
            _animator = new(graph, aimIK, rbody, world, groundDetector, locomotionCore, targetLocker.TargetChangeDuration, _definitions, _animationDefinitions, weaponControlInput);
            _behaviour = new(_definitions, _animator);



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
        public override void Update()
        {
            _behaviour.Update();
            Cursor.lockState = _targetLocker.MainLockTarget == null ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
