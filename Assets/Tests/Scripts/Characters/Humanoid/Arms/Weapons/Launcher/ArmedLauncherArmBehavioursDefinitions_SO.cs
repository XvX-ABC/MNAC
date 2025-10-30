using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    [CreateAssetMenu(fileName = "ArmedLauncherArmBehavioursDefinitions", menuName = "Tests/Definitions/Characters/Humanoid/Arms/Weapons/Launchers/ArmedLauncherArmBehavioursDefinitions")]
    internal class ArmedLauncherArmBehavioursDefinitions_SO : Behaviours.Arms.Weapons.Launchers.ArmedLauncherArmBehavioursDefinitions_SO, IArmedLauncherArmBehaviourDefinitions
    {
        ArmedLauncherArmBehavioursDefinitions _definitions;
        public ITargetsCatcherDefinitions TargetsCatcher => _definitions.TargetsCatcher;

        public ITargetsCatcherDefinitions_V0 CircleOnScreenTargetsCatcher => _definitions.CircleOnScreenTargetsCatcher;
    }
}
