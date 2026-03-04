using System;

namespace MNAC.Utilities.Composable
{
    public interface IComponentDescriptions
    {
        public string Name { get; }
        public Guid ID { get; }
    }
}
