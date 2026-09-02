using System.Runtime.Serialization;

namespace MNAC.Interaction.Influences
{
    public interface IInfluence
    {
        public bool Enabled { get; }
        public string Name { get; }
        public void Update();
    }
}
