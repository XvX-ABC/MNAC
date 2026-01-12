namespace Tests.Interaction.Influences
{
    public abstract class InfluenceBase : IInfluence
    {
        protected bool enabled;

        public virtual bool Enabled { get => enabled; set => enabled = value; }
        public abstract string Name { get; }

        public virtual void Update()
        {
        }

    }
}
