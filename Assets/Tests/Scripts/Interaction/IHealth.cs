using NUnit.Framework;

namespace Tests.Interaction
{
    public interface IHealth : INumerical
    {
        public bool IsAlive { get; }
    }
}
