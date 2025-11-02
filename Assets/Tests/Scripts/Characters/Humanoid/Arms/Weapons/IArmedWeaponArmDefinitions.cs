namespace Tests.Characters.Humanoid.Arms.Weapons
{
    public interface IArmedWeaponArmDefinitions : Behaviours.Arms.Weapons.IArmedWeaponArmDefinitions
    {
        IArmedWeaponArmBehaviour[] ArmedWeaponBehaviours { get; }
    }
}
