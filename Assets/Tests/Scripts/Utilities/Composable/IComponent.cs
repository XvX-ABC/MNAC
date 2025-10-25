using System;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;

namespace Tests.Utilities.Composable
{
    public interface IComponent : IComponentDescriptions, IDisposable
    {
        public bool Enabled { get; set; }
        public Blackboard Blackboard { get; set; }
        public ICharacterComponentNode Node { get; }
        public void Initialize(Blackboard blackboard);

    }
}
