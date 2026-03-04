namespace MNAC.Interaction
{
    public interface IStun
    {
        public bool Enabled { get; }
        public void Begin(float length);
        public void EndEarly();
    }
}
