using System;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    internal class ArmedLauncherArmBehavioursDefinitions_MonoComponent : Behaviours.Arms.Weapons.Launcher.ArmedLauncherArmBehavioursDefinitions_MonoComponent_Obsolete, IArmedLauncherArmBehaviourDefinitions
    {
        [Obsolete]
        [SerializeField]
        TargetsCatcherDefinitions targetsCatcher;
        [SerializeField]
        CircleOnScreenTargetsCatcherDefinitions targetsCatcherV0;
        [Obsolete]
        public ITargetsCatcherDefinitions TargetsCatcher => targetsCatcher;

        public ICircleOnScreenTargetsCatcherDefinitions CircleOnScreenTargetsCatcher => targetsCatcherV0;

        public ITargetLockerDefinitions TargetLocker => throw new NotImplementedException();
    }
}
