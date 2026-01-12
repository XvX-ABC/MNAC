using System;

namespace Tests.Characters.Humanoid
{
    internal class BodyPartNotFoundException : Exception
    {
        public BodyPartNotFoundException(string message) : base(message)
        {

        }
    }
}
