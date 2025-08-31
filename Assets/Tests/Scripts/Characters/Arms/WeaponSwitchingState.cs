using System;
using Tests.Behaviours.Animations;
using Tests.Behaviours.Arms;
using Tests.Behaviours.Arms.Weapons;
using Tests.States;
using Tests.Weapons;
using UnityEngine;

namespace Tests.Characters.Arms
{
    internal class WeaponSwitchingState : StateComponentNode, IAnimationPlayableState, IArmBehaviour
    {


        WeaponSwitching _switching;

        internal Behaviours.Arms.Animations.ArmAnimationCore animationCore;

        protected internal WeaponSwitchingState(WeaponSwitching switching) : base("weapon_switching", 0)
        {
            if (switching == null)
                throw new ArgumentNullException(nameof(switching));
            timeline = switching.timeline;
            _switching = switching;
        }

        public WeaponSwitchingState(IArmWeaponDefinitions definitions, MountPoint mountPoint, WeaponCore weaponCore, Func<WeaponDescription[], string> selectionFunc = null) : this(new(definitions, mountPoint, weaponCore, selectionFunc))
        {
        }


        public Func<IWeapon, IWeapon, IWeapon> SwitchingEvent
        {
            get => _switching.SwitchingEvent;
            set => _switching.SwitchingEvent = value;
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
            timeline.Restart();
            if (animationCore != null)
                animationCore.StatusNum = 0;
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            timeline.OnUpdate(Time.deltaTime);
        }
        public override void OnExit()
        {
            timeline.End();
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
    }
}
