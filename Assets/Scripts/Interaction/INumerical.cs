namespace MNAC.Interaction
{
    public interface INumerical
    {
        public const float MINPOINT = 0;
        public float MaxPoint { get; }
        public float MinPoint { get; }
        public float Point { get; }
        public void ReceivePoint(float point);
    }
}
