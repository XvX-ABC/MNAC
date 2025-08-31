using BehaviorDesigner.Runtime.Tasks;
using RootMotion.FinalIK;
using System;
using Tests.Behaviours.Arms;
using Tests.Behaviours.Arms.Weapons;
using Tests.Behaviours.Arms.Weapons.Launchers;
using Tests.Input;
using Tests.States;
using Tests.Weapons;

namespace Tests.Characters.Arms.Weapons.Launchers
{
    [RequiredComponent(typeof(AimIK))]
    internal class ArmedLauncherArmBehaviour : ArmedWeaponArmBehaviourBase_MonoComponent
    {
        Behaviours.Arms.Weapons.Launchers.ArmedLauncherArmBehaviour _behaviour;
        TargetsCatcher_Debug _targetsCatcher;
        public override WeaponType Type => WeaponType.Launcher;

        public override IPlayableState<object> StateNode => behaviour.StateNode;


        public override IArmedWeaponArmAnimationPlayablePart Animator => behaviour.Animator;

        public override Func<bool> EntryFunc => behaviour.EntryFunc;

        public override Func<bool> ExitFunc => behaviour.ExitFunc;

        public override IWeapon Weapon { get => behaviour.Weapon; set => behaviour.Weapon = value; }
        protected override Behaviours.Arms.Weapons.ArmedWeaponArmBehaviourBase behaviour
        {
            get
            {
                if (_behaviour == null)
                    throw new Exception("This component node is not initialized, so don't try to get any element of this node.");
                return _behaviour;
            }
        }
        protected override void Awake()
        {
            base.Awake();

            var definitions = GetComponent<IArmedLauncherArmBehaviourDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IArmedLauncherArmBehaviourDefinitions));
            var aim = GetComponent<AimIK>() ?? throw new ComponentCantFindException(this.gameObject, typeof(AimIK));

            _targetsCatcher = new();
            _behaviour = new(definitions, aim, _targetsCatcher);
        }
        protected virtual void Update()
        {
            _targetsCatcher.OnUpdate();
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);

            if (blackboard.Contains(CharacterBlackboardFields.TargetsCatcher))
                blackboard.TryWriteValue(CharacterBlackboardFields.TargetsCatcher, _targetsCatcher);
            else
                blackboard.TryRegisterField(CharacterBlackboardFields.TargetsCatcher, _targetsCatcher);

            if (blackboard == null)
                throw new ArgumentNullException(nameof(blackboard));

            blackboard.TryReadValue<IInput>(CharacterBlackboardFields.Input, out var input);

            _behaviour.Input = input;



        }

    }
}
