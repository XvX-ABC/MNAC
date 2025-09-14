using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launchers
{
    public class ArmedLauncherArmBehavioursDefinitions : MonoBehaviour, IArmedLauncherArmBehaviourDefinitions
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
