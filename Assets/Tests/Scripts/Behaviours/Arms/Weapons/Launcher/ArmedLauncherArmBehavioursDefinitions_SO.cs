using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launcher
{
    public class ArmedLauncherArmBehavioursDefinitions_SO : ScriptableObject, IArmedLauncherArmBehaviourDefinitions
    {
        [SerializeField]
        ArmedLauncherArmBehavioursDefinitions _definitions;

        public AnimationClip AimingClip => _definitions.AimingClip;

        public AnimationClip ReloadClip => _definitions.ReloadClip;

        public float IdleAndAimTransitionLength => _definitions.IdleAndAimTransitionLength;

        public float AimAndReloadTransitionLength => _definitions.AimAndReloadTransitionLength;
    }
}
