namespace Tests.TPhysics.Locomotion
{
    public interface ILocomotionModule
    {
        public bool Enabled { get; set; }
        public World World { get; set; }
        public Context Start(Context context);
        public Context Update(Context context);
        public Context End(Context context);
    }
}