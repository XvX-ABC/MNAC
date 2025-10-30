using System;
using Tests.Interaction;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    public interface IArmedLauncherArmBehaviourDefinitions : Behaviours.Arms.Weapons.IArmedLauncherArmBehaviourDefinitions
    {
        [Obsolete]
        public ITargetsCatcherDefinitions TargetsCatcher { get; }
        public ITargetsCatcherDefinitions_V0 CircleOnScreenTargetsCatcher { get; }
    }
}
