namespace Tests.Locomotion
{

    public interface IAirModule : IModule
    {
        public enum State
        {
            OnGround,
            Preparing,
            Ascending,
            Descending
        }
        public State CurrentState { get; set; }
    }
}