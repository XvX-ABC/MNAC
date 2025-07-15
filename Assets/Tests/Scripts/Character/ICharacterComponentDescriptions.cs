using System;

namespace Tests.Characters
{
    public interface ICharacterComponentDescriptions
    {
        public string Name { get; }
        public Guid ID { get; }
    }
}
