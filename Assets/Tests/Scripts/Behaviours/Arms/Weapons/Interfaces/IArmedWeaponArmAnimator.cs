namespace Tests.Behaviours.Arms.Weapons.Animations
{
    internal interface IArmedWeaponArmAnimator
    {
        public ArmedWeaponPlayablePart PlayablePart { get; }
        void OnUpdate();
    }
}
