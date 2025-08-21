using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons
{
    public interface IArmedLauncherArmBehaviourDefinitions
    {
        public AnimationClip AimingClip { get; }
        public AnimationClip ReloadClip { get; }
        public float IdleAndAimTransitionLength { get; }
        public float AimAndReloadTransitionLength { get; }
    }
}
