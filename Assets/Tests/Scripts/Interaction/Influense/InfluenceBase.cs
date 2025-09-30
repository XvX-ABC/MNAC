namespace Tests.Interaction.Influence
{
    public abstract class InfluenceBase : Influence
    {
        protected bool enabled;

        public virtual bool Enabled { get => enabled; set => enabled = value; }
        public abstract string Name { get; }

        public virtual void Update()
        {
        }

    }
}
