using Tests.Weapons;

namespace Tests.Behaviours.Arms.Weapons
{

    public interface IArmedWeaponArmDefinitions
    {
        public WeaponDescription[] Origins { get; }
        public string MountPointName { get; }
        public float SwitchingDurationTime { get; }
        public float SwitchingMountedProportion { get; }
        public float SwitchingToBehavioursDurationTime { get; }
    }
}
