using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    [CreateAssetMenu(fileName = "ArmedLauncherArmBehavioursDefinitions", menuName = "Tests/Definitions/Characters/Humanoid/Arms/Weapons/Launchers/ArmedLauncherArmBehavioursDefinitions")]
    internal class ArmedLauncherArmBehavioursDefinitions_SO : ScriptableObject, IArmedLauncherArmBehaviourDefinitions
    {
        [SerializeField]
        ArmedLauncherArmBehavioursDefinitions _definitions;
        public ITargetsCatcherDefinitions TargetsCatcher => _definitions.TargetsCatcher;

        public ICircleOnScreenTargetsCatcherDefinitions CircleOnScreenTargetsCatcher => _definitions.CircleOnScreenTargetsCatcher;

        public AnimationClip AimingClip => _definitions.AimingClip;

        public AnimationClip ReloadClip => _definitions.ReloadClip;

        public float IdleAndAimTransitionLength => _definitions.IdleAndAimTransitionLength;

        public float AimAndReloadTransitionLength => _definitions.AimAndReloadTransitionLength;
    }
}
