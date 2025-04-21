namespace Tests.BodyBehaviour.Arm
{
    public interface IArmWeaponDefinitions
    {
        public ArmWeaponDescription[] Origins { get; }
        public string MountPointName { get; }
        public float SwitchingDurationTime { get; }
        public float SwitchingMountedProportion { get; }
    }
}
