using System;
using MNAC.Characters.Humanoid.Arms.Weapons.Launchers;
using MNAC.Interaction;
using UnityEngine;

namespace MNAC.Behaviours.Arms.Weapons.Launcher
{
    [Obsolete]
    public class ArmedLauncherArmBehavioursDefinitions_MonoComponent_Obsolete : MonoBehaviour, IArmedLauncherArmBehaviourDefinitions
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

        public LayerMask LayerMaskToHit => throw new System.NotImplementedException();

        public TeamMask TeamMask => throw new System.NotImplementedException();

        public TargetInteraction TargetInteraction => throw new System.NotImplementedException();
    }
}
