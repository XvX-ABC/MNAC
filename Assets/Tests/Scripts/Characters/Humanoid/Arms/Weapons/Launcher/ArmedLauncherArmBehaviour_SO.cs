using RootMotion.FinalIK;
using System;
using Tests.Behaviours.Arms.Weapons;
using Tests.Behaviours.Arms.Weapons.Launcher;
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
using Tests.Weapons;
using Tests.Weapons.Launcher;
using UnityEngine;
using UnityEngine.Playables;
using IndicatedTarget = Tests.Behaviours.Arms.Weapons.Launcher.IndicatedTarget;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    [CreateAssetMenu(fileName = "ArmedLauncherArmBehaviour", menuName = "Tests/Behaviours/Characters/Humanoid/Arms/Weapons/Launchers/ArmedLauncherArmBehaviour")]
    public class ArmedLauncherArmBehaviour_SO : ArmedWeaponArmBehaviourBase_SO
    {
        ArmedLauncherArmAnimator _animator;
        Behaviours.Arms.Weapons.Launcher.ArmedLauncherArmBehaviour _behaviour;
        [SerializeField]
        ArmedLauncherArmBehavioursDefinitions_SO _definitions;
        [SerializeField]
        ArmedLauncherArmAnimationDefinitions_SO _animationDefinitions;
        TargetLocker _targetLocker;
        GameObjsInScreenCatcher_New _screenCatcher;
        IndicatorsManager _indicatorsManager;
        //TargetLocker _targetLocker;
        ArmCore _armCore;
        public override WeaponType Type => WeaponType.Launcher;

        public override IWeapon Weapon
        {
            get => _behaviour.Weapon;
            set
            {
                _behaviour.Weapon = value;
                if (value is ILauncher launcher)
                {
                    var definitions = launcher.Definitions;
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
                    UpdateTargetsCatcherFor(blackboard);
                    _behaviour.Activated = value;
                    _targetLocker.Enabled = value;
                }
                Cursor.visible = !value;
                if (!value)
                    Cursor.lockState = CursorLockMode.None;
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
            blackboard.TryReadValueOrThrowException<Camera>(CharacterBlackboardFields.Character_Camera_Main, out var camera);
            blackboard.TryReadValueOrThrowException<GameObject>(CharacterBlackboardFields.Character_Obj_Main, out var actorObj);
            blackboard.TryReadValueOrThrowException<GameObject>(CharacterBlackboardFields.Character_Obj_Arm_Local, out var armObj);
            blackboard.TryReadValueOrThrowException<ArmCore>(CharacterBlackboardFields.Character_Arm_Core_Local, out _armCore);

            blackboard.TryReadValueOrThrowException<IHumanInput>(CharacterBlackboardFields.Character_Input_Main, out var input);


            //blackboard.TryReadUIValueOrThrowException<RingCatcher>(CharacterUIBlackboardFields.Catcher_Ring, out var ringCatcher);
            //blackboard.TryReadUIValueOrThrowException<TargetsDisplay>(CharacterUIBlackboardFields.Targets_Display, out var targetsDisplay);
            blackboard.TryReadUIValueOrThrowException<ICursorIndicator>(CharacterUIBlackboardFields.Character_Actor_Cursor_Indicator, out var cursorIndicator);
            blackboard.TryReadUIValueOrThrowException<IndicatorsManager>(CharacterUIBlackboardFields.Indicators_Manager, out _indicatorsManager);
            var aimIK = armObj.GetComponent<AimIK>();

            var armInput = Part switch
            {
                HumanPart.LeftArm => input.LArm,
                HumanPart.RightArm => input.RArm,
                _ => null
            };
            //_targetsCatcher = new(camera, input.BaseInput, actorObj, ringCatcher, targetsDisplay, _definitions.CircleOnScreenTargetsCatcher, Activated);


            var weaponControlInput = armInput.WeaponControl;
            //_animator = new(graph, _aimIK, rbody, world, groundDetector, locomotionCore, _definitions, _animationDefinitions, _targetsCatcher, weaponControlInput);
            //_behaviour = new(_definitions, _animator);
            //_behaviour.TargetsCatcher = _targetsCatcher;


            InitializeTargetLocker(input, camera, cursorIndicator);
            InitializeBehaviourAndAnimator(graph, aimIK, camera, input.BaseInput, rbody, world, groundDetector, locomotionCore, armInput.WeaponControl);
        }
        void InitializeTargetLocker(IHumanInput input, Camera camera, ICursorIndicator cursorIndicator)
        {
            var definitions = _definitions.TargetLocker;
            _screenCatcher = new(camera, definitions.HandleAmountInCoroutine);
            _targetLocker = new(input, _screenCatcher, CreateLockTarget, ReleaseLockTarget, camera, cursorIndicator, definitions.ObstacleDetector, definitions.HandleAmountInCoroutine, definitions.CatchAngle, definitions.TargetChangedDuration, definitions.ReceiveInputDuration);
        }
        void InitializeBehaviourAndAnimator(
            PlayableGraph graph,
            AimIK aimIK,
            Camera camera,
            IBaseInput baseInput,
            Rigidbody rbody,
            World world,
            IGroundDetector groundDetector,
            LocomotionCore locomotionCore,
            IWeaponControlInput weaponControlInput)
        {
            _animator = new(graph, aimIK, rbody, world, groundDetector, locomotionCore, _definitions.TargetLocker.TargetChangedDuration, _definitions, _animationDefinitions, weaponControlInput);
            _behaviour = new(_definitions, _animator);
            _behaviour.Input = weaponControlInput;
            _behaviour.TargetLocker = _targetLocker;
        }
        LockTarget CreateLockTarget(GameObject obj, LockType lockType)
        {
            var result = LockTarget.GetInstance(obj, lockType);
            var it = _indicatorsManager.AddTargetFor<IndicatedTarget>(obj);
            result.indicatedTarget = it;
            return result;
        }
        void ReleaseLockTarget(LockTarget target)
        {
            _indicatorsManager.RemoveTargetFor<IndicatedTarget>(target.Obj);
            LockTarget.ReleaseInstance(target);
        }
        public override void OnEnter()
        {
            base.OnEnter();
            //_armCore.StartCoroutine(_targetsCatcher.FilterUpdateWithCoroutine());
            //_armCore.StartCoroutine(_targetsCatcher.CatcherUpdateWithCoroutine());
            _armCore.StartCoroutine(_screenCatcher.UpdateWithCoroutine());
        }
        public override void OnExit()
        {
            //_armCore.StopCoroutine(_targetsCatcher.FilterUpdateWithCoroutine());
            //_armCore.StopCoroutine(_targetsCatcher.CatcherUpdateWithCoroutine());
            base.OnExit();
        }
        public override void Dispose()
        {
            base.Dispose();
            //blackboard.TryUnregisterField(CharacterBlackboardFields.TargetsCatcher, _targetsCatcher);


            //blackboard.TryUnregisterField(CharacterBlackboardFields.TargetsCatcher);
        }
        void UpdateTargetsCatcherFor(Blackboard blackboard)
        {
            //if (Activated)
            //{
            //    blackboard.TryRegisterFieldOrWriteValue(CharacterBlackboardFields.TargetsCatcher, _targetLocker);
            //}
            //else
            //{
            //    blackboard.TryUnregisterField(CharacterBlackboardFields.TargetsCatcher);
            //}


            //if (Activated)
            //    blackboard.TryRegisterFieldOrWriteValue(CharacterBlackboardFields.TargetsCatcher, _targetsCatcher);
            //else
            //    blackboard.TryUnregisterField(CharacterBlackboardFields.TargetsCatcher);
        }
        public override void Update()
        {
            //if (UnityEngine.Input.GetKeyDown(KeyCode.V))
            //    _targetsCatcher.Enabled = !_targetsCatcher.Enabled;
            //_targetsCatcher.LateUpdate();
            _behaviour.Update();
            Cursor.lockState = _targetLocker.MainLockTarget == null ? CursorLockMode.None : CursorLockMode.Locked;
        }
        public override void LateUpdate()
        {
            _targetLocker.LateUpdate();
        }
        public override void FixedUpdate()
        {
            _targetLocker.FixedUpdate();
            //_targetsCatcher.Update();

        }
    }
}
