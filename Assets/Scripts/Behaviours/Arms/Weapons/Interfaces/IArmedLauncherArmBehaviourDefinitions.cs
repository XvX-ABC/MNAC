using System;
using MNAC.Behaviours.Arms.Weapons.Launcher;
using MNAC.Interaction;
using UnityEngine;

namespace MNAC.Behaviours.Arms.Weapons
{
    public interface IArmedLauncherArmBehaviourDefinitions
    {
        //[Obsolete]
        //public AnimationClip AimingClip { get; }
        //[Obsolete]
        //public AnimationClip ReloadClip { get; }
        //public float IdleAndAimTransitionLength { get; }
        //public float AimAndReloadTransitionLength { get; }
        TargetInteraction TargetInteraction { get; }
        LayerMask LayerMaskToHit { get; }
        TeamMask TeamMask { get; }
    }
}
