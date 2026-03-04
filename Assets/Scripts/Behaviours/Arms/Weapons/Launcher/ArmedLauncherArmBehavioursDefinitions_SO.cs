using MNAC.Interaction;
using UnityEngine;

namespace MNAC.Behaviours.Arms.Weapons.Launcher
{
    public class ArmedLauncherArmBehavioursDefinitions_SO : ScriptableObject, IArmedLauncherArmBehaviourDefinitions
    {
        [SerializeField]
        ArmedLauncherArmBehavioursDefinitions _definitions;

        //public AnimationClip AimingClip => _definitions.AimingClip;

        //public AnimationClip ReloadClip => _definitions.ReloadClip;

        //public float IdleAndAimTransitionLength => _definitions.IdleAndAimTransitionLength;

        //public float AimAndReloadTransitionLength => _definitions.AimAndReloadTransitionLength;

        public LayerMask LayerMaskToHit => _definitions.LayerMaskToHit;

        public TeamMask TeamMask => _definitions.TeamMask;

        public TargetInteraction TargetInteraction =>_definitions.TargetInteraction;
    }
}
