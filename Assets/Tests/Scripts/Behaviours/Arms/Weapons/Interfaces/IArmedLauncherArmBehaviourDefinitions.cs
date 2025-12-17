using System;
using Tests.Behaviours.Arms.Weapons.Launcher;
using Tests.Interaction;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons
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
