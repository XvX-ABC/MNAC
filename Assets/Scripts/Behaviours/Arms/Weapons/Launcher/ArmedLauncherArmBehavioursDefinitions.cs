using System;
using Tests.Interaction;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launcher
{
    [Serializable]
    public class ArmedLauncherArmBehavioursDefinitions : IArmedLauncherArmBehaviourDefinitions
    {
        [SerializeField]
        TargetInteraction _targetInteraction;
        [SerializeField]
        LayerMask _layerMaskToHit;
        [SerializeField]
        TeamMask _teamMask;

        public LayerMask LayerMaskToHit { get => _layerMaskToHit; set => _layerMaskToHit = value; }
        public TeamMask TeamMask { get => _teamMask; set => _teamMask = value; }

        public TargetInteraction TargetInteraction => _targetInteraction;

    }
}
