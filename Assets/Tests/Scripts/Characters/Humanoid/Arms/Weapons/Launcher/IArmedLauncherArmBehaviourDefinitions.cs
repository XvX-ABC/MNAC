using System;
using Tests.Interaction;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    public interface IArmedLauncherArmBehaviourDefinitions : Behaviours.Arms.Weapons.IArmedLauncherArmBehaviourDefinitions
    {
        [Obsolete]
        public ITargetsCatcherDefinitions TargetsCatcher { get; }
        public ICircleOnScreenTargetsCatcherDefinitions CircleOnScreenTargetsCatcher { get; }
    }
}
