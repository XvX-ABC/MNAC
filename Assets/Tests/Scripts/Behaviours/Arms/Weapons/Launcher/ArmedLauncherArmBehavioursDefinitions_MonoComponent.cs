using Tests.Characters.Humanoid.Arms.Weapons.Launchers;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launcher
{
    public class ArmedLauncherArmBehavioursDefinitions_MonoComponent : MonoBehaviour, IArmedLauncherArmBehaviourDefinitions
    {
        [SerializeField]
        protected AnimationClip aimingClip;
        [SerializeField]
        protected AnimationClip reloadClip;
        [SerializeField]
        protected float idleAndAimTransitionLength;
        [SerializeField]
        protected float aimAndReloadTransitionsLength;
        public AnimationClip AimingClip => aimingClip;
        public AnimationClip ReloadClip => reloadClip;

        public float IdleAndAimTransitionLength => idleAndAimTransitionLength;

        public float AimAndReloadTransitionLength => aimAndReloadTransitionsLength;
    }
}
