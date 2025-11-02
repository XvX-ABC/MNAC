using System;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    [Serializable]
    internal class ArmedLauncherArmBehavioursDefinitions : Behaviours.Arms.Weapons.Launchers.ArmedLauncherArmBehavioursDefinitions, IArmedLauncherArmBehaviourDefinitions
    {
        [Obsolete]
        TargetsCatcherDefinitions targetsCatcher;
        [SerializeField]
        CircleOnScreenTargetsCatcherDefinitions targetsCatcherV0;
        [Obsolete]
        public ITargetsCatcherDefinitions TargetsCatcher => targetsCatcher;

        public ICircleOnScreenTargetsCatcherDefinitions CircleOnScreenTargetsCatcher => targetsCatcherV0;
    }
}
