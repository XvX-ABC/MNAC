using System;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    internal class ArmedLauncherArmBehavioursDefinitions_MonoComponent : Behaviours.Arms.Weapons.Launchers.ArmedLauncherArmBehavioursDefinitions_MonoComponent, IArmedLauncherArmBehaviourDefinitions
    {
        [Obsolete]
        [SerializeField]
        TargetsCatcherDefinitions targetsCatcher;
        [SerializeField]
        TargetsCatcherDefinitions_V0 targetsCatcherV0;
        [Obsolete]
        public ITargetsCatcherDefinitions TargetsCatcher => targetsCatcher;

        public ITargetsCatcherDefinitions_V0 CircleOnScreenTargetsCatcher => targetsCatcherV0;
    }
}
