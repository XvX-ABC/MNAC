using NUnit.Framework;

namespace MNAC.Interaction
{
    public interface IHealth : INumerical
    {
        public bool IsAlive { get; }
    }
}
