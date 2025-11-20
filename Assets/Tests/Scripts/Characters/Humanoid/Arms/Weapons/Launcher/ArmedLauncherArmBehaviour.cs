using BehaviorDesigner.Runtime.Tasks;
using RootMotion.FinalIK;
using System;
using Tests.Behaviours.Arms.Weapons;
using Tests.Behaviours.Arms.Weapons.Launcher.Animations;
using Tests.Characters.Humanoid.Arms.Weapons;
using Tests.Characters.Humanoid.Interaction.Input;
using Tests.Characters.Humanoid.Locomotion;
using Tests.Characters.UI;
using Tests.TPhysics;
using Tests.TPhysics.Environment;
using Tests.UI;
using Tests.Utilities.Blackboards;
using Tests.Weapons;
using Tests.Weapons.Launcher;
using UnityEngine;
using UnityEngine.Playables;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    [RequiredComponent(typeof(AimIK))]
    public class ArmedLauncherArmBehaviour : ArmedWeaponArmBehaviourBase_MonoComponent
    {
        ArmedLauncherArmAnimator _animator;
        Behaviours.Arms.Weapons.Launcher.ArmedLauncherArmBehaviour _behaviour;
        IArmedLauncherArmBehaviourDefinitions _definitions;
        IArmedLauncherArmAnimationDefinitions _animationDefinitions;

        AimIK _aimIK;
        CircleOnScreenTargetsCatcher _targetsCatcher;
        public override WeaponType Type => WeaponType.Launcher;

        public override IWeapon_Obsolete Weapon
        {
            get => _behaviour.Weapon;
            //set => _behaviour.Weapon = value;
            set
            {
                _behaviour.Weapon = value;
                if (value is ILauncher_Obsolete launcher)
                {
                    var definitions = launcher.Definitions;
                    _targetsCatcher.CatchingRadius = definitions.TargetLock.ViewPortRadius;
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
                base.Activated = value;
                //_targetsCatcher.Enabled = value;
                UpdateTargetsCatcherFor(blackboard);
                _targetsCatcher.Enabled = value;
            }
        }
        protected override void Awake()
        {
            base.Awake();

            _definitions = GetComponent<IArmedLauncherArmBehaviourDefinitions>() ?? throw new ComponentCantFindException(gameObject, typeof(IArmedLauncherArmBehaviourDefinitions));
            _animationDefinitions = GetComponent<IArmedLauncherArmAnimationDefinitions>() ?? throw new ComponentCantFindException(gameObject, typeof(IArmedLauncherArmAnimationDefinitions));
            _aimIK = GetComponent<AimIK>();
            //_targetsCatcher = new(_definitions.TargetsCatcher);
            _targetsCatcher = new(_definitions.CircleOnScreenTargetsCatcher);
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

            //blackboard.TryReadValue<IInput>(CharacterBlackboardFields.Character_Input_Main, out var input);
            blackboard.TryReadValueOrThrowException<IHumanInput>(CharacterBlackboardFields.Character_Input_Main, out var input);


            blackboard.TryReadUIValueOrThrowException<RingCatcher>(CharacterUIBlackboardFields.Catcher_Ring, out var ringCatcher);
            blackboard.TryReadUIValueOrThrowException<TargetsDisplay>(CharacterUIBlackboardFields.Targets_Display, out var targetsDisplay);


            var armInput = default(IArmInput);
            if (Part == HumanPart.LeftArm)
                armInput = input.LArm;
            else if (Part == HumanPart.RightArm)
                armInput = input.RArm;

            _targetsCatcher = new(camera, input.BaseInput, actorObj, ringCatcher, targetsDisplay, _definitions.CircleOnScreenTargetsCatcher, Activated);

            //this.node.AddChild(_targetsCatcher.node);

            var weaponControlInput = armInput.WeaponControl;

            //_animator = new(graph, _aimIK, rbody, world, groundDetector, locomotionCore, _definitions, _animationDefinitions, _targetsCatcher, weaponControlInput);
            //_behaviour = new(_definitions, _animator);
            //_behaviour.TargetsCatcher = _targetsCatcher;

            _behaviour.Input = weaponControlInput;

            StartCoroutine(_targetsCatcher.FilterUpdateWithCoroutine());
            StartCoroutine(_targetsCatcher.CatcherUpdateWithCoroutine());
        }
        public override void Dispose()
        {
            base.Dispose();
            blackboard.TryUnregisterField(CharacterBlackboardFields.TargetsCatcher, _targetsCatcher);
        }
        void UpdateTargetsCatcherFor(Blackboard blackboard)
        {
            if (Activated)
                WriteTargetsCatcherTo(blackboard);
            else
                blackboard.TryUnregisterField(CharacterBlackboardFields.TargetsCatcher);
        }
        void WriteTargetsCatcherTo(Blackboard blackboard)
        {
            if (!blackboard.TryWriteValue(CharacterBlackboardFields.TargetsCatcher, _targetsCatcher))
                blackboard.TryRegisterField(CharacterBlackboardFields.TargetsCatcher, _targetsCatcher);
        }
        private void Update()
        {
            //if (UnityEngine.Input.GetKeyDown(KeyCode.V))
            //    _targetsCatcher.Enabled = !_targetsCatcher.Enabled;
            _behaviour.Update();
            _targetsCatcher.LateUpdate();
        }
        private void FixedUpdate()
        {
            //_targetsCatcher.Update();

        }
    }
}
