using UnityEngine;

namespace Tests.Characters.Arms.Weapons.Launchers
{
    internal class ArmedLauncherArmBehavioursDefinitions : Behaviours.Arms.Weapons.Launchers.ArmedLauncherArmBehavioursDefinitions, IArmedLauncherArmBehaviourDefinitions
    {
        [SerializeField]
        TargetsCatcherDefinitions targetsCatcher;
        public ITargetsCatcherDefinitions TargetsCatcher => targetsCatcher;
    }
}
