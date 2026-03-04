using System;

namespace MNAC.Characters.Interaction
{
    public interface ICharacter : IGuidable
    {
        public string Name { get; }
    }
}
