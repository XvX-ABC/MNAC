using System.Runtime.Serialization;

namespace Tests.Interaction.Influence
{
    public interface IInfluence
    {
        public bool Enabled { get; }
        public string Name { get; }
        public void Update();
    }
}
