namespace Tests.Interaction.Influence
{
    public abstract class InfluenceReceptorBase : IInfluenceReceptor
    {
        protected bool enabled;

        public virtual bool Enabled { get => enabled; set => enabled = value; }
        public abstract string Name { get; }

        public virtual void Update()
        {
        }

        public abstract bool TryGetValue<T>(out T value, object key = null);
        public abstract bool TrySetValue<T>(T value, object key = null);
    }
}
