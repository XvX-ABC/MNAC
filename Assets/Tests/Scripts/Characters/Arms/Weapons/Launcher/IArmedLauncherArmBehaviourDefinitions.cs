using Tests.Interaction;

namespace Tests.Characters.Arms.Weapons.Launchers
{
    public interface IArmedLauncherArmBehaviourDefinitions : Behaviours.Arms.Weapons.IArmedLauncherArmBehaviourDefinitions
    {
        public ITargetsCatcherDefinitions TargetsCatcher { get; }
        public ITargetsCatcherDefinitions_V0 TargetsCatcher_V0 { get; }
    }
}
