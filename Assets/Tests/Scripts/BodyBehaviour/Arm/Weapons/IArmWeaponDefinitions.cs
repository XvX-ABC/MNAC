namespace Tests.Behaviours.Arm
{

    public interface IArmWeaponDefinitions
    {
        public WeaponDescription[] Origins { get; }
        public string MountPointName { get; }
        public float SwitchingDurationTime { get; }
        public float SwitchingMountedProportion { get; }
        public float SwitchingToBehavioursDurationTime { get; }
    }
}
