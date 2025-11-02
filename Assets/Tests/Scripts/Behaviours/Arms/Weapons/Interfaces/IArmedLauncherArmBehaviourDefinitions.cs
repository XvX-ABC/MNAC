using System;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons
{
    public interface IArmedLauncherArmBehaviourDefinitions
    {
        [Obsolete]
        public AnimationClip AimingClip { get; }
        [Obsolete]
        public AnimationClip ReloadClip { get; }
        public float IdleAndAimTransitionLength { get; }
        public float AimAndReloadTransitionLength { get; }
    }
}
