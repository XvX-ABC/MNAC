namespace Tests.Interaction.Influence
{
    public interface IInfluenceReceptor
    {
        public bool Enabled { get; set; }
        public string Name { get; }
        public bool TryGetValue<T>(out T value, object key = null);
        public bool TrySetValue<T>(T value, object key = null);
        public void Update();
    }
}
