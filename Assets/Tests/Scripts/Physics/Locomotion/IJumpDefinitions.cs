using System;

namespace Tests.TPhysics.Locomotion
{
    public interface IJumpDefinitions
    {
        public float Height { get; }
        public float PreparationDuration { get; }
        [Obsolete]
        public float LandingDuration { get; }

    }
}
