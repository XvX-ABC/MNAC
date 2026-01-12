using System;

namespace Tests.Interaction.Influences
{
    public class InfluenceNotExistInCoreException<T> : Exception where T : IInfluence
    {
        public InfluenceNotExistInCoreException()
        {
        }

        public InfluenceNotExistInCoreException(string message) : base(message)
        {
        }
        public InfluenceNotExistInCoreException(T influence) : this($"The influence core is not exist a influence which type is '{typeof(T)}'")
        {

        }
    }
}
