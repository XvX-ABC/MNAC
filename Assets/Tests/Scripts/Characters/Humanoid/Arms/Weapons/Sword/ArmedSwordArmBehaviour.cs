using System;
using Tests.Animations;
using Tests.Behaviours.Arms.Weapons.Sword;
using Tests.Behaviours.Arms.Weapons.Sword.Animations;
using Tests.Characters.Humanoid.Interaction.Input;
using Tests.Characters.Interaction.Input;
using Tests.Characters.MountPoints;
using Tests.Interaction.Targets;
using Tests.States;
using Tests.Utilities.Blackboards;
using Tests.Utilities.MountPoints;
using Tests.Weapons_New;
using UnityEngine;
using UnityEngine.Playables;
using LocomotionCore = Tests.Characters.Humanoid.Locomotion.LocomotionCore;
namespace Tests.Characters.Humanoid.Arms.Weapons.Sword
{
    public class ArmedSwordArmBehaviour : ArmedWeaponArmBehaviourBase_MonoComponent
    {
        IArmedSwordArmBehaviourDefinitions _definitions;
        IArmedSwordArmAnimationDefinitions _animationDefinitions;

        Behaviours.Arms.Weapons.Sword.ArmedSwordArmBehaviour _behaviour;
        ArmedSwordArmAnimator _animator;
        SphereTriggerTargetsCatcher_Obsolete _targetsCatcher;
        LoadBase _load;
        public override WeaponType Type => WeaponType.Sword;

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
                UpdateTargetsCatcherFor(blackboard);
                _targetsCatcher.enabled = value;
            }
        }
        protected override void Awake()
        {
            base.Awake();
            _definitions = GetComponent<IArmedSwordArmBehaviourDefinitions>() ?? throw new ComponentCantFindException(gameObject, typeof(IArmedSwordArmBehaviourDefinitions));
            _animationDefinitions = GetComponent<IArmedSwordArmAnimationDefinitions>() ?? throw new ComponentCantFindException(gameObject, typeof(IArmedSwordArmAnimationDefinitions));
            CreateSphereTriggerTargetsCatcher();
            _load = new(_targetsCatcher.gameObject);
        }
        public override void Update()
        {
            base.Update();
            _behaviour?.Update();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            _behaviour.FixedUpdate();
        }
        void UpdateTargetsCatcherFor(Blackboard blackboard)
        {
            if (Activated)
                WriteTargetsCatcherTo(blackboard);
            else
                blackboard.TryUnregisterField(CharacterBlackboardFields.Character_Component_TargetLocker);
        }
        void WriteTargetsCatcherTo(Blackboard blackboard)
        {
            if (!blackboard.TryWriteValue(CharacterBlackboardFields.Character_Component_TargetLocker, _targetsCatcher))
                blackboard.TryRegisterField(CharacterBlackboardFields.Character_Component_TargetLocker, _targetsCatcher);
        }
        void CreateSphereTriggerTargetsCatcher()
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.SetParent(transform, false);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            _targetsCatcher = obj.AddComponent<SphereTriggerTargetsCatcher_Obsolete>();
            _targetsCatcher.ExcludeLayers = _definitions.ExcludeLayerMask;
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);

            //if (blackboard.Contains(CharacterBlackboardFields.TargetsCatcher))
            //    blackboard.TryWriteValue(CharacterBlackboardFields.TargetsCatcher, _targetsCatcher);
            //else
            //    blackboard.TryRegisterField(CharacterBlackboardFields.TargetsCatcher, _targetsCatcher);

            blackboard.TryReadValueOrThrowException<LocomotionCore>(CharacterBlackboardFields.Character_Locomotion_Core, out var locomotionCore);
            //blackboard.TryReadValueOrThrowException<IInput>(CharacterBlackboardFields.Character_Input_Main, out var input);
            blackboard.TryReadValueOrThrowException<IHumanInput>(CharacterBlackboardFields.Character_Input_Main, out var input);
            blackboard.TryReadValueOrThrowException<Camera>(CharacterBlackboardFields.Character_Camera_Main, out var camera);
            blackboard.TryReadValueOrThrowException<PlayableGraph>(CharacterBlackboardFields.Character_Animation_Graph, out var graph);
            blackboard.TryReadValueOrThrowException<ControllerPlayable>(CharacterBlackboardFields.Character_Animation_Whole_Body_Animator, out var controller);


            InitializeTargetsCatcher(blackboard);

            _definitions.InitializeBy(locomotionCore.definitions);

            var rotationLocker = new RotationLocomotionLocker(locomotionCore.rotation);

            //var boostingHelper = new BoostingHelper(locomotionCore.core, camera, input, _definitions.Boosting);
            var winput = default(IWeaponControlInput);
            if (Part == HumanPart.LeftArm)
                winput = input.LArm?.WeaponControl;
            else if (Part == HumanPart.RightArm)
                winput = input.RArm?.WeaponControl;
            //var boostingHelper = new BoostingHelper(locomotionCore.core, camera, input.BaseInput, winput, _definitions.Boosting);
            var boostingHelper = default(BoostingHelper);
            var slashHelper = new SlashHelper(locomotionCore.core, rotationLocker, _definitions.Slash.Duration, _definitions.Slash.RecoveryDuration);
            var mixer = new WholeBodyMixerPlayable(graph, 3);
            _animator = new(
                graph,
                controller,
                mixer,
                locomotionCore.core,
                locomotionCore.definitions.Walking.MaxSpeed,
                locomotionCore.definitions.Walking.AcceleratedSpeed,
                boostingHelper,
                slashHelper,
                _definitions,
                _animationDefinitions);

            _behaviour = new(_definitions, boostingHelper, slashHelper, _animator);
            //_behaviour.TargetsTrigger = _targetsCatcher;


            var _swordBoostingState = new SwordBoosting(boostingHelper);
            var _swordSlashState = new SwordSlash(slashHelper);


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


        }
        void InitializeTargetsCatcher(Blackboard blackboard)
        {
            blackboard.TryGetMountPointOrThrowException(MountPointFields.Right_Chest_Trigger, out var mountPoint);
            mountPoint.Load = _load;
        }
        public override void Dispose()
        {
            base.Dispose();
            blackboard.TryUnregisterField(CharacterBlackboardFields.Character_Component_TargetLocker);
            blackboard.TryGetMountPointOrThrowException(MountPointFields.Right_Chest_Trigger, out var mountPoint);
            mountPoint.Load = null;
        }
    }
}
