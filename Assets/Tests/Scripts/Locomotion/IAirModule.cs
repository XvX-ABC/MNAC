namespace Tests.Locomotion
{

    public interface IAirModule : IModule
    {

        public State CurrentState { get; set; }
    }
}