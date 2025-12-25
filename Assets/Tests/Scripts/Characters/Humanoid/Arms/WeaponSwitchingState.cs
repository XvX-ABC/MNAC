using System;
using Tests.Animations;
using Tests.Behaviours.Arms.Weapons;
using Tests.States;
using Tests.Utilities.Composable;
using Tests.Utilities.MountPoints;
using Tests.Weapons;
using Tests.Weapons_New;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms
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
        [Obsolete]
        public WeaponSwitchingState(IArmedWeaponArmDefinitions definitions, MountPoint launcherMountPoint, MountPoint swordMountPoint, Weapons_New.WeaponCore_Obsolete weaponCore, Func<WeaponDescription[], string> selectionFunc = null) : this(new(definitions, launcherMountPoint, swordMountPoint, weaponCore, selectionFunc))
        {
        }
        public WeaponSwitchingState(IArmedWeaponArmDefinitions definitions, MountPoint launcherMountPoint, MountPoint swordMountPoint, WeaponBackpack weaponBackpack, Func<WeaponDescription[], string> selectionFunc = null) : this(new(definitions, launcherMountPoint, swordMountPoint, weaponBackpack, selectionFunc))
        {
        }

        public Func<IWeapon, IWeapon, IWeapon> SwitchingEvent
        {
            get => switching.SwitchingEvent;
            set => switching.SwitchingEvent = value;
        }


        IAnimationPlayablePartNode IAnimationPlayableState.Node => animationCore?.switching.Node;


        public override void OnEnter()
        {
            base.OnEnter();
            if (timeline.IsRunning)
            {
                Debug.LogWarning("This weapon switching behaviour is still continuing");
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
