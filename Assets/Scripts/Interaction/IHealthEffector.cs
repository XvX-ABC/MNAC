namespace MNAC.Interaction
{
    public interface IHealthEffector
    {
        public float MaxPoint { get; set; }
        public float MinPoint { get; set; }
        public float Point { get; set; }
        public bool Enabled { get; set; }
    }
}
