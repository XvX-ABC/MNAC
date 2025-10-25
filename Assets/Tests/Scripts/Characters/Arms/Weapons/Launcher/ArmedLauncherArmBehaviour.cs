using BehaviorDesigner.Runtime.Tasks;
using RootMotion.FinalIK;
using System;
using Tests.Behaviours.Arms.Weapons;
using Tests.Behaviours.Arms.Weapons.Launchers.Animations;
using Tests.Characters.Locomotion;
using Tests.Input;
using Tests.TPhysics;
using Tests.TPhysics.Environment;
using Tests.Utilities.Blackboards;
using Tests.Weapons;
using Tests.Weapons.Launcher;
using UnityEngine;
using UnityEngine.Playables;

namespace Tests.Characters.Arms.Weapons.Launchers
{
    [RequiredComponent(typeof(AimIK))]
    public class ArmedLauncherArmBehaviour : ArmedWeaponArmBehaviourBase_MonoComponent
    {
        ArmedLauncherArmAnimator _animator;
        Behaviours.Arms.Weapons.Launchers.ArmedLauncherArmBehaviour _behaviour;
        IArmedLauncherArmBehaviourDefinitions _definitions;
        IArmedLauncherArmAnimationDefinitions _animationDefinitions;

        AimIK _aimIK;
        ScreenCircleTargetsCatcher _targetsCatcher;
        public override WeaponType Type => WeaponType.Launcher;

        public override IWeapon Weapon
        {
            get => _behaviour.Weapon;
            //set => _behaviour.Weapon = value;
            set
            {
                _behaviour.Weapon = value;
                if (value is ILauncher launcher)
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

            _definitions = GetComponent<IArmedLauncherArmBehaviourDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IArmedLauncherArmBehaviourDefinitions));
            _animationDefinitions = GetComponent<IArmedLauncherArmAnimationDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IArmedLauncherArmAnimationDefinitions));
            _aimIK = GetComponent<AimIK>();
            //_targetsCatcher = new(_definitions.TargetsCatcher);
            _targetsCatcher = new(_definitions.TargetsCatcher_V0);
        }

        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);

            if (!blackboard.TryReadValue<Rigidbody>(CharacterBlackboardFields.Rigidbody, out var rbody))
                throw new Exception();
            if (!blackboard.TryReadValue<World>(CharacterBlackboardFields.World, out var world))
                throw new Exception();
            if (!blackboard.TryReadValue<IGroundDetector>(CharacterBlackboardFields.GroundDetector, out var groundDetector))
                throw new Exception();
            if (!blackboard.TryReadValue<LocomotionCore>(CharacterBlackboardFields.Character_Locomotion_Core, out var locomotionCore))
                throw new Exception();
            if (!blackboard.TryReadValue<PlayableGraph>(CharacterBlackboardFields.Character_Animation_Graph, out var graph))
                throw new Exception();
            blackboard.TryReadValue<IInput>(CharacterBlackboardFields.Character_Input_Main, out var input);

            this.node.AddChild(_targetsCatcher.node);

            _animator = new(graph, _aimIK, rbody, world, groundDetector, locomotionCore, _definitions, _animationDefinitions, _targetsCatcher, input);
            _behaviour = new(_definitions, _animator);
            _behaviour.TargetsCatcher = _targetsCatcher;
            _behaviour.Input = input;

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
            if (this.Activated)
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
            _targetsCatcher.Update();
        }
        private void FixedUpdate()
        {
            //_targetsCatcher.Update();
            _behaviour.FixedUpdate();
        }
    }
}
