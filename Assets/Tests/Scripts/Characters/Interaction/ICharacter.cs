using System;

namespace Tests.Characters.Interaction
{
    public interface ICharacter
    {
        public Guid ID { get; }
        public string Name { get; }
    }
}
