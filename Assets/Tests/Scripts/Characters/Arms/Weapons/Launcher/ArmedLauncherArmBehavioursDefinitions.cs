using UnityEngine;

namespace Tests.Characters.Arms.Weapons.Launchers
{
    internal class ArmedLauncherArmBehavioursDefinitions : Behaviours.Arms.Weapons.Launchers.ArmedLauncherArmBehavioursDefinitions, IArmedLauncherArmBehaviourDefinitions
    {
        [SerializeField]
        TargetsCatcherDefinitions targetsCatcher;
        [SerializeField]
        TargetsCatcherDefinitions_V0 targetsCatcherV0;
        public ITargetsCatcherDefinitions TargetsCatcher => targetsCatcher;

        public ITargetsCatcherDefinitions_V0 TargetsCatcher_V0 => targetsCatcherV0;
    }
}
