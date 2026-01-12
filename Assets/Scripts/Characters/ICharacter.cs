using System;

namespace Tests.Characters.Interaction
{
    public interface ICharacter : IGuidable
    {
        public string Name { get; }
    }
}
