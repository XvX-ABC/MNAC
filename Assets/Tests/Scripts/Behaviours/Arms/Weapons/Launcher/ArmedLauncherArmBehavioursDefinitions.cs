using System;
using Tests.Interaction;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launcher
{
    [Serializable]
    public class ArmedLauncherArmBehavioursDefinitions : IArmedLauncherArmBehaviourDefinitions
    {
        [SerializeField]
        LayerMask _layerMaskToHit;
        [SerializeField]
        TeamMask _teamMask;
        //[SerializeField]
        //protected AnimationClip aimingClip;
        ////[SerializeField]
        //protected AnimationClip reloadClip;
        //[SerializeField]
        //protected float idleAndAimTransitionLength;
        //[SerializeField]
        //protected float aimAndReloadTransitionsLength;
        ////[Obsolete]
        //public AnimationClip AimingClip => aimingClip;
        //[Obsolete]
        //public AnimationClip ReloadClip => reloadClip;

        //public float IdleAndAimTransitionLength => idleAndAimTransitionLength;

        //public float AimAndReloadTransitionLength => aimAndReloadTransitionsLength;

        public LayerMask LayerMaskToHit { get => _layerMaskToHit; set => _layerMaskToHit = value; }
        public TeamMask TeamMask { get => _teamMask; set => _teamMask = value; }
    }
}
