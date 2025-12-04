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
using Tests.Characters.Weapons;
using Tests.TPhysics;
using Tests.TPhysics.Environment;
using Tests.UI;
using Tests.Utilities.Blackboards;
using Tests.Weapons.Launcher;
using Tests.Weapons_New;
using UnityEngine;
using UnityEngine.Playables;
using TargetLocker = Tests.Characters.Weapons.TargetLocker;
using WeaponType = Tests.Weapons_New.WeaponType;

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
        ITargetLocker _targetLocker;
        IndicatorsManager _indicatorsManager;
        ArmController _armCore;
        public override WeaponType Type => WeaponType.Launcher;

        public override IWeapon Weapon
        {
            get => _behaviour.Weapon;
            set
            {
                _behaviour.Weapon = value;
                if (value is ILauncher_Obsolete launcher)
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
                    _behaviour.Activated = value;
                    _targetLocker.Enabled = value;
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
            blackboard.TryReadValueOrThrowException<Camera>(CharacterBlackboardFields.Character_Camera_Main, out var camera);
            blackboard.TryReadValueOrThrowException<GameObject>(CharacterBlackboardFields.Character_Obj_Main, out var actorObj);
            blackboard.TryReadValueOrThrowException<GameObject>(CharacterBlackboardFields.Character_Obj_Arm_Local, out var armObj);
            blackboard.TryReadValueOrThrowException<ArmController>(CharacterBlackboardFields.Character_Arm_Core_Local, out _armCore);

            blackboard.TryReadValueOrThrowException<IHumanInput>(CharacterBlackboardFields.Character_Input_Main, out var input);


            blackboard.TryReadUIValueOrThrowException<ICursorIndicator>(CharacterUIBlackboardFields.Character_Actor_Cursor_Indicator, out var cursorIndicator);
            blackboard.TryReadUIValueOrThrowException<IndicatorsManager>(CharacterUIBlackboardFields.Indicators_Manager, out _indicatorsManager);
            var aimIK = armObj.GetComponent<AimIK>();

            var armInput = Part switch
            {
                HumanPart.LeftArm => input.LArm,
                HumanPart.RightArm => input.RArm,
                _ => null
            };


            var weaponControlInput = armInput.WeaponControl;
            blackboard.TryReadValueOrThrowException(CharacterBlackboardFields.TargetLocker, out _targetLocker);
            InitializeBehaviourAndAnimator(graph, aimIK, camera, input.BaseInput, rbody, world, groundDetector, locomotionCore, armInput.WeaponControl);
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
        public override void Update()
        {
            _behaviour.Update();
            Cursor.lockState = _targetLocker.MainLockTarget == null ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
