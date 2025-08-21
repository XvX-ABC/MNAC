using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launchers
{
    public class ArmedLauncherArmBehavioursDefinitions : MonoBehaviour, IArmedLauncherArmBehaviourDefinitions
    {
        [SerializeField]
        AnimationClip _aimingClip;
        [SerializeField]
        AnimationClip _reloadClip;
        [SerializeField]
        float _idleAndAimTransitionLength;
        [SerializeField]
        float _aimAndReloadTransitionsLength;
        public AnimationClip AimingClip => _aimingClip;
        public AnimationClip ReloadClip => _reloadClip;

        public float IdleAndAimTransitionLength => _idleAndAimTransitionLength;

        public float AimAndReloadTransitionLength => _aimAndReloadTransitionsLength;
    }
}
