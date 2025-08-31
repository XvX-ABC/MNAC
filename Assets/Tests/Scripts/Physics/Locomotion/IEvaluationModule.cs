namespace Tests.TPhysics.Locomotion
{
    public interface IEvaluationModule
    {
        public World World { get; set; }
        public bool Enabled { get; set; }
        public Context Update(Context context);
    }
}