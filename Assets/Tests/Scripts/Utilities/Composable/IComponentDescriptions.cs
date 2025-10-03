using System;

namespace Tests.Utilities.Composable
{
    public interface IComponentDescriptions
    {
        public string Name { get; }
        public Guid ID { get; }
    }
}
