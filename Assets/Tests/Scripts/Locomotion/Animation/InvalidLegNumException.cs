using System;

namespace Tests.Locomotion.Animation
{
    class InvalidLegNumException : Exception
    {
        public InvalidLegNumException(string message) : base(message)
        {

        }
        public InvalidLegNumException() : this("The leg num can only be 0 or 1.")
        {

        }
    }
}