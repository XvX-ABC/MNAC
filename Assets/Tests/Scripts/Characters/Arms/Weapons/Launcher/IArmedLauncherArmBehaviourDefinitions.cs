namespace Tests.Characters.Arms.Weapons.Launchers
{
    public interface IArmedLauncherArmBehaviourDefinitions : Behaviours.Arms.Weapons.IArmedLauncherArmBehaviourDefinitions
    {
        public ITargetsCatcherDefinitions TargetsCatcher { get; }
    }
}
