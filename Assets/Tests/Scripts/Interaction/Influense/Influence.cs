using System.Runtime.Serialization;

namespace Tests.Interaction.Influence
{
    public interface Influence
    {
        public bool Enabled { get; }
        public string Name { get; }
        public void Update();
    }
}
