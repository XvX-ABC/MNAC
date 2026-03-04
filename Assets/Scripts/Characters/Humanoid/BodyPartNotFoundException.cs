using System;

namespace MNAC.Characters.Humanoid
{
    internal class BodyPartNotFoundException : Exception
    {
        public BodyPartNotFoundException(string message) : base(message)
        {

        }
    }
}
