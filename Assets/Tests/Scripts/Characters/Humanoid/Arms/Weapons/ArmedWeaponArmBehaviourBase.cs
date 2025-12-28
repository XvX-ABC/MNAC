using System;
using Tests.Behaviours.Arms.Weapons;
using Tests.Characters.Humanoid.Arms.Weapons.Launchers;
using Tests.Characters.Humanoid.Arms.Weapons.Sword;
using Tests.States;
using Tests.Utilities.Composable;
using Tests.Weapons;
using Tests.Weapons_New;
using WeaponType = Tests.Weapons_New.WeaponType;

namespace Tests.Characters.Humanoid.Arms.Weapons
{
    public abstract class ArmedWeaponArmBehaviourBase
        : StateComponentNode, IArmedWeaponArmBehaviour
    {

        public static ArmedWeaponArmBehaviourBase CreateBehaviour(WeaponType weaponType, bool enabled = true)
        {
            return weaponType switch
            {
                WeaponType.Launcher => new ArmedLauncherArmBehaviour("armed_launcher_arm_behaviour", enabled),
                WeaponType.Sword => new ArmedSwordArmBehaviour("armed_sword_arm_behaviour", enabled),
                _ => throw new Exception("Invalid weapon type")
            };
        }
        HumanBodyPart _part;

        protected ArmedWeaponArmBehaviourBase(string name, bool enabled = true) : base(name, 0, enabled)
        {
        }

        protected abstract Behaviours.Arms.IArmedWeaponArmBehaviour behaviour { get; }
        public virtual bool Activated
        {
            get => enabled;
            set
            {
                enabled = value;
                behaviour.Activated = value;
            }
        }

        public abstract WeaponType Type { get; }
        public virtual IWeapon Weapon { get => behaviour.Weapon; set => behaviour.Weapon = value; }
        public virtual IArmedWeaponArmAnimationPlayablePart Animator { get => behaviour.Animator; }
        public virtual Func<bool> EntryFunc { get => behaviour.EntryFunc; }
        public virtual Func<bool> ExitFunc { get => behaviour.ExitFunc; }
        public HumanBodyPart Part
        {
            get
            {
                if (_part != HumanBodyPart.LeftArm && _part != HumanBodyPart.RightArm)
                    throw new ArgumentException("ArmedWeaponArmBehaviourBase_MonoComponent can only be attached to LeftArm or RightArm");
                return _part;
            }
            set => _part = value;
        }
        public override void OnEnter()
        {
            base.OnEnter();
            behaviour?.State?.OnEnter();
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            behaviour?.State?.OnUpdate();
        }
        public override void OnExit()
        {
            base.OnExit();
            behaviour?.State?.OnExit();
        }

        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            behaviour?.State?.FromPreviousStateTransitionBegin(currentTransition);
        }

        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            behaviour?.State?.FromPreviousStateTransitionRunning(currentTransition);
        }
        public override void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionEnd(currentTransition);
            behaviour?.State?.FromPreviousStateTransitionEnd(currentTransition);
        }
        public override void ToNextStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionBegin(currentTransition);
            behaviour?.State?.ToNextStateTransitionBegin(currentTransition);
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            behaviour?.State?.ToNextStateTransitionRunning(currentTransition);
        }
        public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionEnd(currentTransition);
            behaviour?.State?.ToNextStateTransitionEnd(currentTransition);
        }

        public virtual void BehaviourOnUpdate()
        {
        }

        public virtual void BehaviourOnLateUpdate()
        {

        }
        public virtual void BehaviourOnFixedUpdate()
        {
        }
    }
}
