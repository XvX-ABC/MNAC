using Assets.Tests.Scripts.BodyBehaviour.Arm.Animations;

namespace Tests.BodyBehaviour.Arm.Animations
{
    public interface IArmWeaponAnimationDefinitions
    {
        public IArmWeaponSwitchingAnimationDefinitions Switching { get; }
    }
    public interface IArmAnimationDefinitions
    {
        public IArmWeaponAnimationDefinitions Weapon { get; }
    }
}
