using System;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launchers
{
    [Serializable]
    public class ArmedLauncherArmBehavioursDefinitions : IArmedLauncherArmBehaviourDefinitions
    {
        //[SerializeField]
        protected AnimationClip aimingClip;
        //[SerializeField]
        protected AnimationClip reloadClip;
        [SerializeField]
        protected float idleAndAimTransitionLength;
        [SerializeField]
        protected float aimAndReloadTransitionsLength;
        [Obsolete]
        public AnimationClip AimingClip => aimingClip;
        [Obsolete]
        public AnimationClip ReloadClip => reloadClip;

        public float IdleAndAimTransitionLength => idleAndAimTransitionLength;

        public float AimAndReloadTransitionLength => aimAndReloadTransitionsLength;
    }
}
