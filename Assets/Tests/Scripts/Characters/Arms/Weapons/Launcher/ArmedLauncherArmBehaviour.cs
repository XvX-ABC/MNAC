using BehaviorDesigner.Runtime.Tasks;
using RootMotion.FinalIK;
using System;
using Tests.Behaviours.Arms.Weapons;
using Tests.Input;
using Tests.States;
using Tests.Weapons;

namespace Tests.Characters.Arms.Weapons.Launchers
{
    [RequiredComponent(typeof(AimIK))]
    internal class ArmedLauncherArmBehaviour : ArmedWeaponArmBehaviourBase_MonoComponent
    {
        Behaviours.Arms.Weapons.Launchers.ArmedLauncherArmBehaviour _behaviour;
        //TargetsCatcher_Obsolete _targetsCatcher;
        IArmedLauncherArmBehaviourDefinitions _definitions;
        AimIK _aimIk;
        TargetCatcher _targetsCatcher;
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

            _definitions = GetComponent<IArmedLauncherArmBehaviourDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(IArmedLauncherArmBehaviourDefinitions));
            _aimIk = GetComponent<AimIK>() ?? throw new ComponentCantFindException(this.gameObject, typeof(AimIK));


        }
        protected virtual void Update()
        {
        }
        protected virtual void FixedUpdate()
        {

            _targetsCatcher.Update();
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

            _targetsCatcher = new(_definitions.TargetsCatcher);
            this.node.AddChild(_targetsCatcher.Node);


            _behaviour = new(_definitions, _aimIk, _targetsCatcher);
            _behaviour.Input = input;


        }

    }
}
