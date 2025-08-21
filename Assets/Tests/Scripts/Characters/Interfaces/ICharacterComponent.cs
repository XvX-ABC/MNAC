using System;
using Tests.States;

namespace Tests.Characters
{
    public interface ICharacterComponent : ICharacterComponentDescriptions, IDisposable
    {
        public bool Enabled { get; set; }
        public Blackboard Blackboard { get; set; }
        public ICharacterComponentNode Node { get; }
        public void Initialize(Blackboard blackboard);

    }
}
