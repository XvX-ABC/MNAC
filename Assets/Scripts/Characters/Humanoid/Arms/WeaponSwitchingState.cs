using System;
using MNAC.Animations;
using MNAC.Behaviours.Arms.Weapons;
using MNAC.Characters.Weapons;
using MNAC.States;
using MNAC.Utilities.Composable;
using MNAC.Utilities.MountPoints;
using MNAC.Weapons;
using MNAC.Weapons;
using UnityEngine;
using WeaponBackpack = MNAC.Characters.Weapons.WeaponBackpack;
namespace MNAC.Characters.Humanoid.Arms
{
    internal class WeaponSwitchingState : StateComponentNode, IAnimationPlayableState, IArmBehaviour
    {


        internal WeaponSwitching switching;

        internal Behaviours.Arms.Animations.ArmAnimationCore animationCore;

        protected internal WeaponSwitchingState(WeaponSwitching switching) : base("weapon_switching", 0)
        {
            if (switching == null)
                throw new ArgumentNullException(nameof(switching));
            timeline = switching.timeline;
            this.switching = switching;
        }
        public WeaponSwitchingState(ArmController ownerArmController, IArmedArmDefinitions definitions, MountPoint launcherMountPoint, MountPoint swordMountPoint, WeaponBackpack weaponBackpack, Func<WeaponDescription[], string> selectionFunc = null) : this(new(ownerArmController, definitions, launcherMountPoint, swordMountPoint, weaponBackpack, selectionFunc))
        {
        }

        public Action<IWeapon, IWeapon> SwitchedEvent
        {
            get => switching.SwitchedEvent;
            set => switching.SwitchedEvent = value;
        }


        IAnimationPlayablePartNode IAnimationPlayableState.Node => animationCore?.switching.Node;


        public override void OnEnter()
        {
            base.OnEnter();
            if (timeline.IsRunning)
            {
                Debug.LogWarning("This weapon switching behaviour was still continuing");
                return;
            }
            switching.Begin();
            if (animationCore != null)
                animationCore.StatusNum = 0;
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            switching.Update();
        }
        public override void OnExit()
        {
            switching.End();
            base.OnExit();
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            if (animationCore != null)
            {
                var t = currentTransition.Timeline.NormalizedTime;
                animationCore.SwitchingWeight = t;
            }
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            if (animationCore != null)
            {
                var t = currentTransition.Timeline.NormalizedTime;
                animationCore.SwitchingWeight = 1 - t;
            }
        }
        public void SetDefaultWeapon()
        {
            switching.SetDefaultWeapon();
        }
    }
}
